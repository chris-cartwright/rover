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
using Newtonsoft.Json;

namespace VehicleLib
{
	/// <summary>
	/// Stores three related values. Intended to contain movement information on 3 axis for a vehicle.
	/// Each property calls Validator on value before setting it.
	/// </summary>
	/// <typeparam name="T">Any type; Numeric make most sense.</typeparam>
	[Serializable]
	public class Vector3<T>
	{
		/// <summary>
		/// Available axis
		/// </summary>
		public enum Axis { X = 0, Y, Z };

		/// <summary>
		/// Defaults to no validation
		/// </summary>
		[JsonIgnore]
		public Func<T, T> Validator;

		private T _x;
		private T _y;
		private T _z;
		public T X
		{
			get => _x;
			set => _x = Validator(value);
		}
		public T Y
		{
			get => _y;
			set => _y = Validator(value);
		}
		public T Z
		{
			get => _z;
			set => _z = Validator(value);
		}

		/// <summary>
		/// Default constructor.
		/// </summary>
		public Vector3()
		{
			Validator = t => t;
		}

		/// <summary>
		/// Sets axis to value.
		/// </summary>
		/// <param name="value">Value to set axis to.</param>
		/// <param name="axis">Axis to set.</param>
		public Vector3(T value, Axis axis)
			: this()
		{
			switch (axis)
			{
			case Axis.X:
				X = value;
				break;
			case Axis.Y:
				Y = value;
				break;
			default:
				Z = value;
				break;
			}
		}

		/// <summary>
		/// Sets all axis to values.
		/// </summary>
		/// <param name="x">Value to set X axis to.</param>
		/// <param name="y">Value to set Y axis to.</param>
		/// <param name="z">Value to set Z axis to.</param>
		public Vector3(T x, T y, T z)
			: this()
		{
			X = x;
			Y = y;
			Z = z;
		}

		/// <summary>
		/// Sets an axis to a value. Usefull when axis isn't know until runtime.
		/// </summary>
		/// <param name="axis">Axis whose value to set.</param>
		/// <param name="value">Value to set axis to.</param>
		/// <returns>Value axis was actually set to. Will depend on Validator</returns>
		/// <exception>Thrown when axis is not valid.
		///     <cref>AgumentOutOfRangeException</cref>
		/// </exception>
		public T SetAxis(Axis axis, T value)
		{
			switch (axis)
			{
			case Axis.X:
				X = value;
				return X;

			case Axis.Y:
				Y = value;
				return Y;

			case Axis.Z:
				Z = value;
				return Z;
			}

			throw new ArgumentOutOfRangeException(nameof(axis));
		}
	}

	/// <summary>
	/// Specialize Vector3 to short.
	/// </summary>
	public class SVector3 : Vector3<short>
	{
		public SVector3()
		{
			Validator = delegate(short value)
			{
				if (value < -100)
				{
					value = -100;
				}

				if (value > 100)
				{
					value = 100;
				}

				return value;
			};
		}

		public SVector3(short speed, Axis axis)
			: this()
		{
			switch (axis)
			{
			case Axis.X:
				X = speed;
				break;
			case Axis.Y:
				Y = speed;
				break;
			default:
				Z = speed;
				break;
			}
		}

		public SVector3(short x, short y, short z)
			: this()
		{
			X = x;
			Y = y;
			Z = z;
		}
	}
}
