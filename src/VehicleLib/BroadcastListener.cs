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

namespace VehicleLib
{
	public class BroadcastListener
	{
		public delegate void VehicleBroadcastHandler(string name, IPEndPoint ipEndPoint);

		public event VehicleBroadcastHandler OnBroadcastReceived;

		private JsonLineProtocol _proto = new JsonLineProtocol();
		private Thread _thread;
		private UdpClient _listener;

		public BroadcastListener()
		{
			_thread = new Thread(delegate(object o) { Run((ushort)o); });
		}

		/// <summary>
		/// Wraps Run inside a thread.
		/// </summary>
		/// <param name="port">Port to listen for vehicles on</param>
		public void Start(ushort port)
		{
			_thread.Start(port);
		}

		/// <summary>
		/// Listens for Vehicle Broadcasts using UPD broadcasts on specified port using local machine network settings for broardcast IP.
		/// Raises on event on vehicle broardcast received
		/// </summary>
		/// <param name="listenPort">Port to listen for vehicle broadcasts</param>
		// http://msdn.microsoft.com/en-us/library/tst0kwb1.aspx
		public void Run(ushort listenPort)
		{
			//bool done = false;
			_listener = new UdpClient(listenPort);
			IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);
			Encoding ASCII = Encoding.ASCII;
			try
			{
				while (true)
				{
					// Waiting for broadcast
					byte[] bytes = _listener.Receive(ref groupEP);

					dynamic[] msgs = _proto.Feed(Encoding.ASCII.GetString(bytes));

					// add to event
					string name = "";
					ushort connectionPort;

					foreach (dynamic received in msgs)
					{
						try
						{
							name = received.name;
							connectionPort = received.port;

							IPEndPoint vehicleIPEndPoint = new IPEndPoint(groupEP.Address, connectionPort);
							OnBroadcastReceived(name, vehicleIPEndPoint);
						}
						catch (RuntimeBinderException) { } // caught a broadcast that is not formatted correctly (not from a vehicle)
					}
				}
			}
			catch (SocketException ex)
			{
				// Thread was killed
				if (ex.ErrorCode == 10004)
					return;

				throw new Exceptions.ConnectionException("Error in protocol.", ex);
			}
		}

		/// <summary>
		/// Kills the thread
		/// </summary>
		public void Shutdown()
		{
			if(_listener.Client != null)
				_listener.Client.Close();

			if(_thread.ThreadState == ThreadState.Running)
				_thread.Join();
		}
	}
}
