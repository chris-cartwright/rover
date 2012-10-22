using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleLib
{
	class ConnectionException : VehicleException
	{
		public ConnectionException() { }

		public ConnectionException(string message) : base(message) { }

		public ConnectionException(string message, Exception innerException) : base(message, innerException) { }
	}
}
