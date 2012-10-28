using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleLib
{
    [Serializable]
    public class MoveState : State
    {
        public ushort Speed;

        public MoveState() { }
        public MoveState(ushort spd)
        {
            this.Speed = spd;
        }
    }
}
