﻿/*
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
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using VehicleLib.Exceptions;

namespace VehicleLib
{
	/// <summary>
	/// class VehiclePipe
	/// Library to connect, send commands to and recieve signals from a vehicle.
	/// </summary>
	public class VehiclePipe
	{
		public delegate void ErrorHandler(Errors.Error err);
		public delegate void SensorHandler(Sensors.Sensor sensor);
		public delegate void ExceptionHandler(Exception ex);

		public event ErrorHandler OnError;
		public event Action OnConnect;
		public event Action OnLogin;
		public event Action OnDisconnect;
		public event SensorHandler OnSensorEvent;
		public event ExceptionHandler OnException;

		// Encapsulates callback counter and dictionary logic
		private class CallbackDictionary
		{
			private uint _counter;
			private Dictionary<uint, SensorHandler> _callbacks;

			public CallbackDictionary()
			{
				_counter = 0;
				_callbacks = new Dictionary<uint, SensorHandler>();
			}

			public uint Add(SensorHandler cb)
			{
				if (_counter == UInt32.MaxValue)
					_counter = 0;
				else
					_counter++;

				_callbacks.Add(_counter, cb);
				return _counter;
			}

			public void Remove(uint id)
			{
				_callbacks.Remove(_counter);
			}

			public void Clear()
			{
				_callbacks.Clear();
			}

			public SensorHandler this[uint key]
			{
				get { return _callbacks[key]; }
			}
		}

		private Socket _socket;
		private CallbackDictionary _callbacks;
		private Thread _thread;

		public VehiclePipe()
		{
			_callbacks = new CallbackDictionary();

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

			if(OnDisconnect != null)
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
			SendRaw(q, _callbacks.Add(q.Callback), q.GetType().Name);
		}

		/// <summary>
		/// Different actions have different functionality.
		/// Some Actions may persist a device state until a new state is set, while others may cause a device to 
		/// perform an "instance movement" where the device stops once a state is reaches... think of an arm instructed to extend. 
		/// Exceptions are allowed to bubble
		/// </summary>
		/// <param name="a">Action object to send to Rover</param>	
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
				dynamic packet;

				if (callbackID != null)
					packet = new { cmd = cmd, data = o, id = callbackID };
				else
					packet = new { cmd = cmd, data = o };

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
					OnDisconnect();
				}
				
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
			SendRaw(login, null, "Login");
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
				else if (packet.cmd.ToString().Contains("Sensor"))
					cat = "Sensors.";
				else if (packet.cmd.ToString().Contains("Event"))
					cat = "Events.";

				string finder = "VehicleLib." + cat + packet.cmd + ", VehicleLib";
				Type t = Type.GetType(finder, true);
				MethodInfo cast = typeof(JToken).GetMethod("ToObject", new Type[] {}).MakeGenericMethod(t);
				receivedObject = cast.Invoke(packet.data, null);
			}
			catch (RuntimeBinderException)
			{
				throw new MalformedMessageException() { Malformed = JsonConvert.SerializeObject(packet) };
			}

			if (packet.cmd.ToString().Contains("Error"))
			{
				if (OnError != null)
				{
					OnError((Errors.Error)receivedObject);
				}

				return;
			}

			if (packet.cmd.ToString().Contains("Sensor"))
			{
				SensorHandler cb = null;
				if (packet.id != null)
				{
					uint id = packet.id.ToObject<uint>();
					cb = _callbacks[id];
					_callbacks.Remove(id);
				}

				Sensors.Sensor sens = (Sensors.Sensor)receivedObject;
				if (cb != null)
					cb(sens);
				else
				{
					if (OnSensorEvent != null)
					{
						OnSensorEvent(sens);
					}
				}

				return;
			}

			if (packet.cmd.ToString().Contains("Event"))
			{
				((Events.Event)receivedObject).Invoke();
				return;
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
