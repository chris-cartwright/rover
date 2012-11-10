using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleLib.VehicleException
{
	class ParseFailedException : VehicleException
	{
		public ParseFailedException() : base("Invalid message received, parse failed.") { }
	}
}
