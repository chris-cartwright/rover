using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleLib.VehicleException 
{
	public class NoLoginException : Exception
	{
		public NoLoginException() : base("Not logged in.") { }
	}
}
