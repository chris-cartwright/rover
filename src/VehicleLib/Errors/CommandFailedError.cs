﻿using System;

namespace VehicleLib.Errors
{
	/// <summary>
	/// Return from vehicle when a command fails to execute.
	/// Note that the command did attempt execute, which means it was found.
	/// </summary>
	[Serializable]
	public class CommandFailedError : Error
	{
		/// <summary>
		/// Name of command that failed.
		/// </summary>
		public string Command { get; set; }

		/// <summary>
		/// Error message given from failure.
		/// </summary>
		public string Error { get; set; }

		public CommandFailedError() : base("Command failed to execute.") { }

		public override string ToString()
		{
			return $"{Message} [{Command}, {Error}]";
		}
	}
}
