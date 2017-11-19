/*
    Copyright (C) 2012 Christopher Cartwright
    Copyright (C) 2012 Richard Payne
    Copyright (C) 2012 Andrew Hill
    Copyright (C) 2012 David Shirley
    
    This file is part of VDash.

    VDash is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    VDash is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with VDash.  If not, see <http://www.gnu.org/licenses/>.
*/

using System.Collections.Generic;
using Newtonsoft.Json;

namespace VehicleLib
{
	/// <summary>
	/// Splits incoming data on \r\n then attempts to parse the split strings as JSON.
	/// </summary>
	public class JsonLineProtocol
	{
		public delegate void MessageHandler(dynamic message);
		public event MessageHandler OnMessage;

		private string _data = string.Empty;

		/// <summary>
		/// Feeds data into the internal buffer and checks for line delimiter.
		/// If one is found, parses out the line and tries to convert it to Json.
		/// Continues to loop until no line delimiters are found.
		/// This function is meant to work on both an event and functional model.
		/// </summary>
		/// <param name="str">Data to feed into the internal buffer.</param>
		/// <returns>An array containing any JSON objects found.</returns>
		public dynamic[] Feed(string str)
		{
			_data += str;

			var ret = new List<dynamic>();

			var index = _data.IndexOf("\r\n");
			while (index != -1)
			{
				var command = _data.Substring(0, index);
				_data = _data.Remove(0, index + 2);

				dynamic packet = JsonConvert.DeserializeObject(command.Trim());

				OnMessage?.Invoke(packet);

				ret.Add(packet);

				index = _data.IndexOf("\r\n");
			}

			return ret.ToArray();
		}
	}
}
