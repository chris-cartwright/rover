using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleLib.VehicleException
{
	public class InvalidLoginException : Exception
	{
		public InvalidLoginException() { }

		public InvalidLoginException(string message) : base(message) { }

		public InvalidLoginException(string message, Exception innerException) : base(message, innerException) { }
	}
}
