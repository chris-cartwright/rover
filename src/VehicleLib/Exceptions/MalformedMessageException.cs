using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleLib.Exceptions
{
	public class MalformedMessageException : VehicleException
	{
		public string Malformed { get; set; }

		public MalformedMessageException() { }

		public MalformedMessageException(string message) : base(message) { }

		public MalformedMessageException(string message, string malformed) : base(message) { Malformed = malformed; }

		public MalformedMessageException(string message, Exception innerException) : base(message, innerException) { }
	}
}
