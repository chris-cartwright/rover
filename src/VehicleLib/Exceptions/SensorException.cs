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

using System;

namespace VehicleLib.Exceptions
{
	[Serializable]
	public class SensorException : VehicleException
	{
		// private members
		private readonly uint _id;

		// constructors
		public SensorException() { }

		public SensorException(uint id, string message) : base(message) 
		{
			_id = id;
		}

		public SensorException(uint id,string message, Exception innerException) : base(message, innerException) 
		{
			_id = id;
		}

		// Properties - getters only
		public float GetId()
		{
			return _id;
		}
	}
}
