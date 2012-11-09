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
	public class BroadcastListener
	{
		private bool _quit = false;
		public delegate void VehicleBroadcastHandler(string name, IPEndPoint ipEndPoint);
		public event VehicleBroadcastHandler VehicleBroadcastEvent;
		
		/// <summary>
		/// Listens for Vehicle Broadcasts using UPD broadcasts on specified port using local machine network settings for broardcast IP.
		/// Raises on event on vehicle broardcast received
		/// </summary>
		/// <param name="listenPort">Port to listen for vehicle broadcasts</param>
		// http://msdn.microsoft.com/en-us/library/tst0kwb1.aspx
		public void Start(ushort listenPort)
		{
			//bool done = false;
			UdpClient listener = new UdpClient(listenPort);
			IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);
			Encoding ASCII = Encoding.ASCII;
			try {
				while (!_quit)
				{
					// Waiting for broadcast
					byte[] bytes = listener.Receive(ref groupEP);

					// add to event
					string name = "";
					string ipString = "";
					ushort connectionPort;
					try
					{
						// get string for ip
						dynamic received = JsonConvert.DeserializeObject(bytes.ToString());
						//dynamic received = JsonConvert.DeserializeObject(Encoding.ASCII.GetString(bytes, 0, bytes.Length));
						name = received.name;
						connectionPort = received.port;

						IPEndPoint vehicleIPEndPoint = new IPEndPoint(IPAddress.Parse(ipString), connectionPort);
						VehicleBroadcastEvent(name, vehicleIPEndPoint);
					}
					catch { } // caught a broad cast that is not formatted correctly (not from a vehicle)
				}
			}
			catch (Exception ex)
			{
				throw new ConnectionException("Failed to set up Vehicle Listener.", ex);
			}
			finally{
				listener.Close();
			}
		}

		public void Quit()
		{
			_quit = true;
		}
	}
}
