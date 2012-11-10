using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleLib.Errors
{
	[Serializable]
	public class ConcurrentConnectionError : Error
	{
		public ConcurrentConnectionError() { }  // consider making default private and exposing to JSON deserializartio only

		public ConcurrentConnectionError(string message) : base(message) { }
	}
}
