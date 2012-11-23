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
	//public enum AxisType { X = 0, Y, Z };

    [Serializable]
    public class MoveState : State
    {
        public short Speed;
		protected DVector Vector { get; set; }

		[Serializable]
		protected class DVector
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

			public DVector(short speed, AxisType axis)
			{
				switch (axis)
				{
				case AxisType.X :
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

			public DVector(short speedX, short speedY, short speedZ)
			{
				X = speedX;
				Y = speedY;
				Z = speedZ;
			}
		} // end class DVector


        public MoveState() { }

		public MoveState(short speed)
        {
			Vector = new DVector(speed, AxisType.X);
        }

		public MoveState(short speedX, short speedY, short speedZ)
		{
			Vector = new DVector(speedX, speedY, speedZ);
		}

		public MoveState(short speed, AxisType axis)
		{
			Vector = new DVector(speed, axis);
		}

		public override string Cmd
		{
			get { return "MoveState"; }
		}
    }
}
