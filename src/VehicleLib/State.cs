using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleLib
{
    [Serializable]
    public class State
    {
        private byte _pwm;
        private bool Enabled;

        public State() { }
        public State(byte bytPwm, bool blnEnabled)
        {
            this._pwm = bytPwm;
            this.Enabled = blnEnabled;
        }

        // Properties
        public byte GetPwm()
        {
            return _pwm;
        }

        public void SetPwm(byte newPwm)
        {
            this._pwm = newPwm;
        }

        public bool GetEnabled()
        {
            return Enabled;
        }

        public void SetEnabled(bool newEnabled)
        {
            this.Enabled = newEnabled;
        }

    }
}
