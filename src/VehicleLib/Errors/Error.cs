﻿/*
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleLib.Errors
{
	/// <summary>
	///  An exception generated by a vehicle.
	///  Because javascript is not capable of generating a proper exception, a JSON object containing a vehicle exception is thrown
	///  This is the base class for all vehicle exceptions
	/// </summary>
	[Serializable]
	public class Error
	{
		private string _message;

		public string Message
		{
			get { return _message; }
		}

		
		public Error() {} // consider making default private and exposing to JSON deserializartio only

		public Error(string message)
		{
			_message = message;
		}
	}
}
