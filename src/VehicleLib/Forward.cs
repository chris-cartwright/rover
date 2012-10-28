using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleLib
{
    [Serializable]
    public class Forward : MoveState
    {
        public Forward() { }  
        public Forward(ushort spd)
        {
            this.Speed = spd;
        }
    }
}
