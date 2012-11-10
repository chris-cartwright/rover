using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleLib.VehicleException
{
	public class CommandNotFoundException : VehicleException
	{
		public string Command {get; set;}

		public CommandNotFoundException() : base("Command not found.") { }
	}
}
