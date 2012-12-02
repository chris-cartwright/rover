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

namespace VehicleLib.States
{
	[Serializable]
	public class MoveState : State
	{
		public SVector3 Vector { get; protected set; }

		public MoveState()
		{
			Vector = new SVector3();
		}

		public MoveState(short speed)
			: this()
		{
			Vector.Z = speed;
		}

		public MoveState(short x, short y, short z)
			: this()
		{
			Vector.X = x;
			Vector.Y = y;
			Vector.Z = z;
		}

		public MoveState(short speed, SVector3.Axis axis)
			: this()
		{
			Vector.SetAxis(axis, speed);
		}

		public override string Cmd
		{
			get { return "MoveState"; }
		}
	}
}
