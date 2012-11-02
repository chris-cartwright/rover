using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace VehicleLib
{
	public class BroadcastListener
	{
		private bool _quit = false;
		public delegate void VehicleBroadcastHandler(string name, IPEndPoint ipEndPoint);
		public event VehicleBroadcastHandler VehicleBroadcastEvent;
		
		/// <summary>Listener
		/// void Start(ushort listenPort)
		/// Listens for UPD broadcasts on specified port using local machine network settings.
		/// Raises on event on detection passing 
		/// </summary>
		/// <param name="listenPort">Port to listen for vehicle broadcasts</param>
		// http://msdn.microsoft.com/en-us/library/tst0kwb1.aspx
		public void Start(ushort listenPort, ushort connectionPort)
		{
			//bool done = false;
			UdpClient listener = new UdpClient(listenPort);
			IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, listenPort);

			try {
				while (!_quit)
				{
					// Waiting for broadcast
					byte[] bytes = listener.Receive(ref groupEP);
					
					//Console.WriteLine("Received broadcast from {0} :\n {1}\n",
					//	groupEP.ToString(),
					//	Encoding.ASCII.GetString(bytes, 0, bytes.Length));

					// check if the broadcast came from a vehicle

					// add to event
					string name = "";
					string ipString = "";
					IPEndPoint vehicleIPEndPoint = new IPEndPoint(IPAddress.Parse(ipString), connectionPort);
					VehicleBroadcastEvent(name, vehicleIPEndPoint);
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
