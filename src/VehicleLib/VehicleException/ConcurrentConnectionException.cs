using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleLib.VehicleException
{
	public class ConcurrentConnectionException : VehicleException
	{
		public ConcurrentConnectionException() : base("Vehicle already has a master.") { }
	}
}
