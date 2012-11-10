using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleLib.Errors 
{
	[Serializable]
	public class NoLoginError : Error
	{
		public NoLoginError() { }  // consider making default private and exposing to JSON deserializartio only
		public NoLoginError(string message) : base(message) { }
	}
}
