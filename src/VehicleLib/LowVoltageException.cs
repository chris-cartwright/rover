using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleLib
{
	class LowVoltageException : VehicleException
	{
		// private members
		private float _voltage;

		// constructors
		public LowVoltageException() { }

		public LowVoltageException(float voltage, string message) : base(message) 
		{
			_voltage = voltage;
		}

		public LowVoltageException(float voltage, string message, Exception innerException) : base(message, innerException) 
		{
			_voltage = voltage;
		}

		// Properties - getters only
		public float Voltage
		{
			get { return _voltage; }
		}
	}
}
