using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VDash
{
    public class Record
    {
        private string _name;
        private object _value;
        private double _timeOffset;

        public Record()
        {

        }
        public Record (string n, string v, double dt)
        {
            _name = n;
            _value = v;
            _timeOffset = dt;
        }

        public string getName()
        {
            return _name;
        }

        public void setName(string n)
        {
            this._name = n;
        }

        public object getValue()
        {
            return _value;
        }

        public void setValue(object v)
        {
            this._value = v;
        }

        public double getTimeOffset()
        {
            return _timeOffset;
        }

        public void setTimeOffset(double t)
        {
            this._timeOffset = t;
        }
    }
}
