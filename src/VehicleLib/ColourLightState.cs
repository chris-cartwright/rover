using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleLib
{
    [Serializable]
    public class ColourLightState : LightState
    {
        public byte Red;
        public byte Green;
        public byte Blue;

        public ColourLightState() { }
        public ColourLightState(byte bytRed, byte bytGreen, byte bytBlue)
        {
            this.Red = bytRed;
            this.Green = bytGreen;
            this.Blue = bytBlue;
        }
    }
}
