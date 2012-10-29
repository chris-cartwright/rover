using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleLib
{
    public class LightSensor : SensorInfo
    {
        public byte Level;

        public LightSensor() { }
        public LightSensor(byte _lvl)
        {
            this.Level = _lvl;            
        }
    }
}
