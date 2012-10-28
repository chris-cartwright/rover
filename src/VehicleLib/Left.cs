using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleLib
{
    [Serializable]
    public class Left : TurnState
    {        
        public Left() { }
        public Left(short pcent)
        {
            this.Percent = pcent;
        }
    }
}
