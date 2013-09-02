/*
Copyright (C) 2013 Christopher Cartwright
    
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

using System;
using System.IO.Ports;

namespace Serial
{
	class Serial
	{
		static void Main()
		{
			Console.Write("Serial port: ");
			string port = Console.ReadLine();

			Console.Write("Baud [9600]: ");

			int baud;
			try
			{
				baud = Int32.Parse(Console.ReadLine() ?? "9600");
			}
			catch (FormatException)
			{
				baud = 9600;
			}

			new Serial(port, baud).Run();
		}

		private readonly SerialPort _serial;

		public Serial(string port, int baud)
		{
			_serial = new SerialPort(port, baud);
		}

		public void Run()
		{
			Console.WriteLine("Type 'QUIT' to exit.");
			Console.WriteLine("Prepend a '0x' to a two digit hex code to send the equivalent byte");

			_serial.Open();
			using (_serial)
			{
				_serial.DataReceived += (sender, e) => Console.WriteLine("\nReceived: {0}\nSend: ", _serial.ReadExisting());

				while (true)
				{
					Console.Write("Send: ");
					string str = Console.ReadLine() ?? "";
					if (str == "QUIT")
					{
						break;
					}

					if (str.Length == 4 && str.Substring(0, 2) == "0x")
					{
						byte b = Convert.ToByte(str.Substring(2), 16);
						_serial.Write(new[] { b }, 0, 1);
					}
					else
					{
						_serial.Write(str);
					}
				}
			}
		}
	}
}
