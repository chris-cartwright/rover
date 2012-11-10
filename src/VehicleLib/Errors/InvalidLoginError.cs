using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleLib.Errors
{
	[Serializable]
	public class InvalidLoginError : Error
	{
		public InvalidLoginError() { }  // consider making default private and exposing to JSON deserializartio only

		public InvalidLoginError(string message) : base(message) { }
	}
}
