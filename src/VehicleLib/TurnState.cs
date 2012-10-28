using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleLib
{
    [Serializable]
    public class TurnState : State
    {
        public short Percent;

        public TurnState() { }
        public TurnState(short pcent)
        {
            this.Percent = pcent;
        }
    }
}
