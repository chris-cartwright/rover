using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleLib.Errors
{
	[Serializable]
	public class CommandFailedError : Error
	{
		public string Command { get; set; }
		public string Error { get; set; }

		public CommandFailedError() : base("Command failed to execute.") { }

		public CommandFailedError(string cmd, string error)
			: base("Command failed to execute.")
		{
			Command = cmd;
			Error = error;
		}

		public override string ToString()
		{
			return String.Format("{0} [{1}, {2}]", Message, Command, Error);
		}
	}
}
