/*
    Copyright (C) 2012 Christopher Cartwright
    Copyright (C) 2012 Richard Payne
    Copyright (C) 2012 Andrew Hill
    Copyright (C) 2012 David Shirley
    
    This file is part of VDash.

    VDash is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    VDash is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with VDash.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Newtonsoft.Json;

namespace VehicleLib
{
	/// <summary>
	/// class VehiclePipe
	/// Library to connect, send commands to and recieve signals from a vehicle.
	/// </summary>
	public class VehiclePipe
	{
		// private members
		public delegate void ErrorHandler(VehicleLib.Errors.Error err);
		public delegate void SensorInfoHandler(SensorInfo si);
		private Socket _socket;
		private Dictionary<uint, SensorInfoHandler> _callbacks;
		private uint _callbackCounter;
		public event ErrorHandler OnError;
		public event System.Action OnDisconnect;
		public event SensorInfoHandler OnSensorEvent;

		// http://msdn.microsoft.com/en-us/library/system.net.sockets.addressfamily.aspx
		/// <summary>
		/// void Connect(IPEndPoint vehicleIPEndPoint, string password)
		/// Connects to a vehicle on a supplied ip and port
		/// Uses any avaible port on local machine
		/// Clears callback functions
		/// Zeros out callback counter
		/// Registers a callback for the login
		/// </summary>
		/// <param name="vehicleIPEndPoint">System.Net.IPEndPoint of the vehicle</param>
		/// <param name="password">string</param>
		public void Connect(IPEndPoint vehicleIPEndPoint, Login login)
		{
			if (_socket != null)
			{
				throw new VehicleException.ConnectionException("Device already controlled/connected to a client.");
			}
			//const string server = "localhost";
			IPEndPoint hostEndPoint;
			IPAddress hostAddress = null;
			Byte[] RecvBytes = new Byte[256];

			// Get DNS host information.
			IPHostEntry hostInfo = Dns.GetHostEntry(vehicleIPEndPoint.Address);
			// Get the DNS IP addresses associated with the host.
			IPAddress[] IPaddresses = hostInfo.AddressList;

			// Evaluate the socket and receiving host IPAddress and IPEndPoint.  
			for (int index = 0; index < IPaddresses.Length; index++)
			{
				hostAddress = IPaddresses[index];
				hostEndPoint = new IPEndPoint(hostAddress, vehicleIPEndPoint.Port);

				// Creates the Socket to send data over a TCP connection.
				_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

				// Connect to the host using its IPEndPoint.
				_socket.Connect(hostEndPoint);

				if (!_socket.Connected)
				{
					// Connection failed, try next IPaddress.
					_socket = null;
					continue;
				}

				_callbacks = new Dictionary<uint, SensorInfoHandler>();
				_callbackCounter = 0;
				_callbacks.Clear();

				Login(login);
				return;
			}
		}

		/// <summary>
		/// Disconnects from a vehicle.
		/// </summary>
		public void Disconnect()
		{
			try
			{
				_socket.Close();

			}
			catch { }
			_socket = null;
			OnDisconnect();
		}

		/// <summary>
		/// Sends a query to a vehicle requesting a sensor value.
		/// Adds a callback to the callback Dictionary
		/// Non-blocking, the order the Queries are sent will not neccessarily be the order the Quiers are returned.
		/// </summary>
		/// <param name="q">Query object to send to Rover</param>
		/// Exceptions are allowed to bubble
		public void Send(Query q)
		{
			if (_callbackCounter == uint.MaxValue)
			{
				_callbackCounter = 0;
			}
			++_callbackCounter;
			_callbacks.Add(_callbackCounter, q.Callback);
			SendRaw(q, _callbackCounter);
		}

		/// <summary>
		/// Different actions have different functionality.
		/// Some Actions may persist a device state until a new state is set, while others may cause a device to 
		/// perform an "instance movement" where the device stops once a state is reaches... think of an arm instructed to extend. 
		/// Exceptions are allowed to bubble
		/// </summary>
		/// <param name="q">Action object to send to Rover</param>	
		public void Send(Action a) // might have to be careful here. There is a System.Action 
		{
			SendRaw(a, null);
		}

		/// <summary>
		/// Send any Serializable oject to a vehicle.
		/// Oject is serialized using JSON JsonConvert.SerializeObject.
		/// </summary>
		/// <param name="o">ojbect to send</param>
		/// <param name="callbackID">Optional parameter, used when sending an object that expects an object to be returned from the vehicle.</param>
		private void SendRaw(object o, uint? callbackID)
		{
			try
			{
				dynamic packet = new { cmd = o.GetType().Name, data = o };

				if (callbackID != null)
				{
					packet.id = callbackID;
				}

				string s = JsonConvert.SerializeObject(packet) + "\r\n";
				Encoding ASCII = Encoding.ASCII;
				Byte[] ByteGet = ASCII.GetBytes(s);
				_socket.Send(ByteGet, ByteGet.Length, 0);
			}
			catch (Exception ex)
			{
				if (!_socket.Connected)
				{
					_socket = null;
				}
				OnDisconnect();
				throw new VehicleException.ConnectionException("Failed to send data.", ex);
			}
		}

		/// <summary>
		/// Single attempt login to a vehicle
		/// Sends special Login packet to vehicle using SendRaw
		/// No callback created, assumed connected as TCP pipe used
		/// </summary>
		/// <param name="password">Password to be sent to Vehicle</param>
		private void Login(Login login)
		{
			Encoding ASCII = Encoding.ASCII;
			SendRaw(login, null);
		}

		/// <summary>
		/// Recieves a string from a vehicle expecting a JSON serialized oject.
		/// Recieved packet must contain a [NameSpace.][packet.cmd]. ex. "VDash." + [packet.cmd]
		/// Calls Command() on JSON deserialize string
		/// </summary>
		/// <param name="packet">Packet received from a vehicle.  Must be proper JSON notation.</param>	
		private void Recv()
		{
			// Receive the host home page content and loop until all the data is received.
			Byte[] RecvBytes = new Byte[256];
			string strRetPage = "";
			Encoding ASCII = Encoding.ASCII;
			Int32 bytes = _socket.Receive(RecvBytes, RecvBytes.Length, 0);

			strRetPage += ASCII.GetString(RecvBytes, 0, bytes);

			while (bytes > 0)
			{
				bytes = _socket.Receive(RecvBytes, RecvBytes.Length, 0);
				strRetPage += ASCII.GetString(RecvBytes, 0, bytes);
			}

			int index = strRetPage.IndexOf("\r\n");
			if (index != -1)
			{
				string command = strRetPage.Substring(0, index);
				strRetPage = strRetPage.Remove(0, index);

				dynamic packet = JsonConvert.DeserializeObject(command.Trim());

				try
				{
					Command(packet);
				}
				catch (TimeoutException)
				{
					// Log error and rethrow
					_socket = null;
					OnDisconnect();
					throw new VehicleException.ConnectionException("Connection timed out.");
				}
				catch (Exception ex)
				{
					// Log error and rethrow				
					_socket = null;
					OnDisconnect();
					throw new VehicleException.ConnectionException("Something went wrong in VehiclePipe.Recv() | " + ex.Message, ex);
				}
			}
		}

		/// <summary>
		/// Accepts a dynamic packet. packet must contain a [NameSpace.][packet.cmd]. ex. "VDash." + [packet.cmd] 
		/// </summary>
		/// <param name="packet">Packet containing a VehicleLib class object (either a SensorInfo or a VehicleException).  Must be proper JSON notation.</param>
		private void Command(dynamic packet)
		{
			object receivedOject = Convert.ChangeType(packet.data, Type.GetType("VDash." + packet.cmd));

			if (packet.cmd.toString().Contains("Error"))
			{
				if (OnError != null)
				{
					OnError((Errors.Error)receivedOject);
				}
			}
			if (packet.id != null)
			{
				UInt32 id = Convert.ToUInt32(packet.id);
				_callbacks[id]((SensorInfo)receivedOject);
				_callbacks.Remove(id);
			}
			else
			{
				SensorInfo sens = (SensorInfo)receivedOject;
				if (OnSensorEvent != null)
				{
					OnSensorEvent(sens);
				}
			}
		}

		// Properties - getters only
		public Socket Socket
		{
			get { return _socket; }
		}

		public bool Connected
		{
			get { return _socket.Connected; }
		}
	} // end class VehiclePipe
} // end namespace
