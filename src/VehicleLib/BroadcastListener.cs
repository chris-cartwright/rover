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

using System.Net;
using System.Net.Sockets;
using System.Text;
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

		/// <summary>
		/// Wraps Run inside a thread.
		/// </summary>
		/// <param name="ep">IP address of interface and port to listen on.</param>
		public void Start(IPEndPoint ep)
		{
			_thread = new Thread(delegate(object o) { Run((IPEndPoint)o); });
			_thread.Start(ep);
		}

		/// <summary>
		/// Listens for Vehicle Broadcasts using UPD broadcasts on specified port using local machine network settings for broardcast IP.
		/// Raises on event on vehicle broardcast received
		/// </summary>
		/// <param name="ep">IP address of interface and port to listen on.</param>
		// http://msdn.microsoft.com/en-us/library/tst0kwb1.aspx
		public void Run(IPEndPoint ep)
		{
			_listener = new UdpClient(ep);
			IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, ep.Port);
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

							if(OnBroadcastReceived != null)
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
			if (_listener == null)
				return;

			if(_listener.Client != null)
				_listener.Client.Close();

			if(_thread.ThreadState == ThreadState.Running)
				_thread.Join();
		}
	}
}
