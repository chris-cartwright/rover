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
using System.Threading;
using Microsoft.CSharp.RuntimeBinder;
using VehicleLib.Exceptions;
using Newtonsoft.Json.Linq;

namespace VehicleLib
{
	/// <summary>
	/// class VehiclePipe
	/// Library to connect, send commands to and recieve signals from a vehicle.
	/// </summary>
	public class VehiclePipe
	{
		public delegate void ErrorHandler(Errors.Error err);
		public delegate void SensorInfoHandler(Sensors.SensorInfo si);
		public delegate void ExceptionHandler(Exception ex);

		public event ErrorHandler OnError;
		public event System.Action OnConnect;
		public event System.Action OnDisconnect;
		public event SensorInfoHandler OnSensorEvent;
		public event ExceptionHandler OnException;

		private Socket _socket;
		private Dictionary<uint, SensorInfoHandler> _callbacks;
		private uint _callbackCounter
		{
			get { return _callbackCounter; }
			set
			{
				if (_callbackCounter == uint.MaxValue)
				{
					_callbackCounter = 0;
				}
			}
		}
		private Thread _thread;

		public VehiclePipe()
		{
			_thread = new Thread(delegate()
			{
				try
				{
					while(true)
						Recv();
				}
				catch (SocketException ex)
				{
					// Thread was killed
					if (ex.ErrorCode == 10004)
						return;

					if (OnException != null)
						OnException(ex);
				}
				catch (Exception ex)
				{
					if (OnException != null)
						OnException(ex);
				}
			});
		}

		// http://msdn.microsoft.com/en-us/library/system.net.sockets.addressfamily.aspx
		/// <summary>
		/// Connects to a vehicle on a supplied ip and port
		/// Clears callback functions
		/// Zeros out callback counter
		/// Registers a callback for the login
		/// This starts a thread for received data.
		/// </summary>
		/// <param name="vehicleIPEndPoint">Socket for vehicle</param>
		/// <param name="password">Credentials used to authenticate connection</param>
		public void Connect(IPEndPoint vehicleIPEndPoint, Login login)
		{
			if (_socket != null)
			{
				throw new Exceptions.ConnectionException("Device already controlled/connected to a client.");
			}

			_socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

			try
			{
				_socket.Connect(vehicleIPEndPoint);
			}
			catch (SocketException ex)
			{
				throw new ConnectionException("Could not connect to vehicle.", ex);
			}

			if (!_socket.Connected)
			{
				_socket = null;
				throw new ConnectionException("Could not connect to vehicle.");
			}

			_callbacks = new Dictionary<uint, SensorInfoHandler>();
			_callbackCounter = 0;
			_callbacks.Clear();

			if (OnConnect != null)
			{
				OnConnect();
			}

			_thread.Start();

			Login(login);
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
			++_callbackCounter;
			_callbacks.Add(_callbackCounter, q.Callback);
			SendRaw(q, _callbackCounter, "Query");
		}

		/// <summary>
		/// Different actions have different functionality.
		/// Some Actions may persist a device state until a new state is set, while others may cause a device to 
		/// perform an "instance movement" where the device stops once a state is reaches... think of an arm instructed to extend. 
		/// Exceptions are allowed to bubble
		/// </summary>
		/// <param name="q">Action object to send to Rover</param>	
		public void Send(States.State a) // might have to be careful here. There is a System.Action 
		{
			SendRaw(a, null, a.Cmd);
		}

		/// <summary>
		/// Send any Serializable oject to a vehicle.
		/// Oject is serialized using JSON JsonConvert.SerializeObject.
		/// </summary>
		/// <param name="o">ojbect to send</param>
		/// <param name="callbackID">Optional parameter, used when sending an object that expects an object to be returned from the vehicle.</param>
		private void SendRaw(object o, uint? callbackID, string cmd)
		{
			try
			{
				dynamic packet = new { cmd = cmd, data = o };

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
				throw new ConnectionException("Failed to send data.", ex);
			}
		}

		/// <summary>
		/// Single attempt login to a vehicle
		/// Sends special Login packet to vehicle using SendRaw
		/// No callback created, assumed connected as TCP pipe used
		/// </summary>
		/// <param name="login">Credentials used for authentication</param>
		private void Login(Login login)
		{
			++_callbackCounter;
			_callbacks.Add(_callbackCounter, null);
			SendRaw(login, _callbackCounter, "Login");
		}

		/// <summary>
		/// Handles incoming data on the pipe.
		/// Feeds the data into an instance of JsonLineProtocol.
		/// Loops over any packets that were decoded and sends them to Command
		/// </summary>
		private void Recv()
		{
			Byte[] RecvBytes = new Byte[256];
			JsonLineProtocol proto = new JsonLineProtocol();
			_socket.Receive(RecvBytes);
			
			dynamic[] msgs = proto.Feed(Encoding.ASCII.GetString(RecvBytes));

			foreach(dynamic packet in msgs)
			{
				try
				{
					Command(packet);
				}
				catch (TimeoutException ex)
				{
					_socket = null;

					if(OnDisconnect != null)
						OnDisconnect();

					throw ex;
				}
				catch (SocketException ex)
				{
					_socket = null;

					if(OnDisconnect != null)
						OnDisconnect();

					throw new ConnectionException("Connection is in an error state.", ex);
				}
			}
		}

		/// <summary>
		/// Accepts a dynamic packet. packet must contain a [NameSpace.][packet.cmd]. ex. "VDash." + [packet.cmd] 
		/// </summary>
		/// <param name="packet">An unknon object received from a vehicle.</param>
		private void Command(dynamic packet)
		{
			object receivedObject;
			try
			{
				string cat = "";
				if (packet.cmd.ToString().Contains("Error"))
					cat = "Errors.";
				else if (packet.cmd.ToString().Contains("SensorInfo"))
					cat = "Sensors.";

				string finder = "VehicleLib." + cat + packet.cmd + ", VehicleLib";
				Type t = Type.GetType(finder, true);
				MethodInfo cast = typeof(JToken).GetMethod("ToObject", new Type[] {}).MakeGenericMethod(t);
				receivedObject = cast.Invoke(packet.data, null);
			}
			catch (RuntimeBinderException)
			{
				throw new MalformedMessageException() { Malformed = JsonConvert.SerializeObject(packet) };
			}

			SensorInfoHandler cb = null;
			if(packet.id != null)
			{
				uint id = packet.id.ToObject<uint>();
				cb = _callbacks[id];
				_callbacks.Remove(id);
			}

			if (packet.cmd.ToString().Contains("Error"))
			{
				if (OnError != null)
				{
					OnError((Errors.Error)receivedObject);
				}

				return;
			}
			
			Sensors.SensorInfo sens = (Sensors.SensorInfo)receivedObject;
			if (cb != null)
				cb(sens);
			else
			{
				if (OnSensorEvent != null)
				{
					OnSensorEvent(sens);
				}
			}
		}

		/// <summary>
		/// Kills the underlying thread.
		/// </summary>
		public void Shutdown()
		{
			if(_socket != null)
				_socket.Close();

			if(_thread.ThreadState == ThreadState.Running)
				_thread.Join();
		}

		// Properties - getters only
		public Socket Socket
		{
			get { return _socket; }
		}

		public bool Connected
		{
			get { return _socket == null ? false : _socket.Connected; }
		}
	} // end class VehiclePipe
} // end namespace
