using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleLib
{
    [Serializable]
    public class LightState : State
    {
        public ushort id;
        public ushort Level;

        public LightState() { }
        public LightState(ushort usId, ushort lvl)
        {
            this.id = usId;
            this.Level = lvl;
        }

    }
}
