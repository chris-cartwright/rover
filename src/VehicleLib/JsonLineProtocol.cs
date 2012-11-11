using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

		private string _data = String.Empty;

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

			List<dynamic> ret = new List<dynamic>();

			int index = _data.IndexOf("\r\n");
			while (index != -1)
			{
				string command = _data.Substring(0, index);
				_data = _data.Remove(0, index + 2);

				dynamic packet = JsonConvert.DeserializeObject(command.Trim());

				if (OnMessage != null)
					OnMessage(packet);

				ret.Add(packet);

				index = _data.IndexOf("\r\n");
			}

			return ret.ToArray();
		}
	}
}
