using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleLib
{
    public class SensorInfo
    {
        public ushort ID;
        public DateTime Time;

        public SensorInfo() { }
        public SensorInfo(ushort _id, DateTime _time)
        {
            this.ID = _id;
            this.Time = _time;
        }
            
    }
}
