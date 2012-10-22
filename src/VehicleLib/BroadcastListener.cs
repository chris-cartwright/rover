using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace VehicleLib
{
	public class BroadcastListener
	{
		private Socket _socket;
		private VehiclePipe _connection;

		public void Start(ushort port)
		{
			_connection.Connect("localhost", port);
		}
	}
}
