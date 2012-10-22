using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleLib
{
	class SensorException : VehicleException
	{
		// private members
		private uint _id;

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
		public float getID()
		{
			return _id;
		}
	}
}
