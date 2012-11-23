/*
    Copyright (C) 2012 Christopher Cartwright
    Copyright (C) 2012 Richard Payne
    Copyright (C) 2012 Andrew Hill
    Copyright (C) 2012 David Shirley
    
    This file is part of VDash.

    VDash is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    VDash is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with VDash.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VehicleLib.States
{
    [Serializable]
    public class TurnState : State
    {
		public short Speed;
		protected OVector Vector { get; set; }

		[Serializable]
		protected class OVector
		{
			private short _x;
			private short _y;
			private short _z;
			public short X
			{
				get { return _x; }
				set
				{
					if (value < -100)
						value = -100;

					if (value > 100)
						value = 100;
					_x = value;
				}
			}
			public short Y
			{
				get { return _y; }
				set
				{
					if (value < -100)
						value = -100;

					if (value > 100)
						value = 100;
					_y = value;
				}
			}
			public short Z
			{
				get { return _z; }
				set
				{
					if (value < -100)
						value = -100;

					if (value > 100)
						value = 100;
					_z = value;
				}
			}

			public OVector(short speed, AxisType axis)
			{
				switch (axis)
				{
				case AxisType.X:
					X = speed;
					break;
				case AxisType.Y:
					Y = speed;
					break;
				default:
					Z = speed;
					break;
				}
			}

			public OVector(short speedX, short speedY, short speedZ)
			{
				X = speedX;
				Y = speedY;
				Z = speedZ;
			}
		} // end class DVector

        public TurnState() { }

		public TurnState(short speed)
        {
			Vector = new OVector(speed, AxisType.X);
        }

		public TurnState(short speedX, short speedY, short speedZ)
		{
			Vector = new OVector(speedX, speedY, speedZ);
		}

		public TurnState(short speed, AxisType axis)
		{
			Vector = new OVector(speed, axis);
		}
		
		public override string Cmd
		{
			get { return "TurnState"; }
		}
    }
}
