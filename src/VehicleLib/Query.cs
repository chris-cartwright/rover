using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleLib
{
    [Serializable]
	public class Query
	{
		// public members
		public delegate void CallbackHandler(SensorInfo si);  //Note from Dave: not sure how this is supposed to work yet

        // private members
        private uint _id;
        private uint _sensor;

		public CallbackHandler Callback;


        // constructors
		public Query() { }

            /*
            public Query(uint id, uint sensor, Delegate cb)
            {
                this._id = id;
                this._sensor = sensor;
                //callBack = cb;
            }
            */
        public Query(uint id, uint sensor)
        {
            this._id = id;
            this._sensor = sensor;
        }

        // Properties
        public uint GetID()
        {
            return _id;
        }

        public void SetID(uint id)
        {
            this._id = id;
        }

        public uint GetSensorID()
        {
            return _sensor;
        }

        public void SetSensorID( uint sensorID)
        {
            this._sensor = sensorID;
        }
	}

    
}
