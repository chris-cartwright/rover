using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleLib
{
    [Serializable]
    public class Back : MoveState
    {
        public Back() { }
        public Back(ushort spd)
        {
            this.Speed = spd;
        }
    }
}
