using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleLib.Errors
{
	[Serializable]
	public class ParseFailedError : Error
	{
		public ParseFailedError() { }  // consider making default private and exposing to JSON deserializartio only

		public ParseFailedError(string message) : base(message) { }
	}
}
