using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleLib
{
	[Serializable]
	public class VehicleException : System.Exception
	{
		public VehicleException () { }

		public VehicleException(string message) : base(message) { }

		public VehicleException(string message, Exception innerException) : base(message, innerException) { }

	}// end VehicleException class
}// end namespace
