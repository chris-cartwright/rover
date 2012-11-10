using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleLib.Errors
{
	[Serializable]
	public class CommandNotFoundError : Error
	{
		public string Command {get; set;}

		public CommandNotFoundError() { }  // consider making default private and exposing to JSON deserializartio only

		public CommandNotFoundError(string message) : base(message) { }

		public CommandNotFoundError(string message, string command) : base(message) 
		{
			Command = command;
		}
	}
}
