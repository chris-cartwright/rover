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
using Newtonsoft.Json;

namespace VehicleLib.Queries
{
	[Serializable]
	public class Query
	{
		public string Sensor { get; set; }

		[JsonIgnore]
		public VehiclePipe.SensorHandler Callback;

		// constructors
		public Query() { }

		public Query(string sensor)
		{
			Sensor = sensor;
		}

		public Query(string sensor, VehiclePipe.SensorHandler callback)
		{
			Sensor = sensor;
			Callback = callback;
		}
	}
}
