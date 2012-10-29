using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleLib
{
    public class TempSensor : SensorInfo
    {
        public short Temp;

        public TempSensor() { }
        public TempSensor(short _temp)
        {
            this.Temp = _temp;
        }
    }
}
