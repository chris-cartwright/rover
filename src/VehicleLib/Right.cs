using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleLib
{
    [Serializable]
    public class Right: TurnState
    {        
        public Right() { }
        public Right(short pcent)
        {
            this.Percent = pcent;
        }
    }
}
