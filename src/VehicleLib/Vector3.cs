using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace VehicleLib
{
	[Serializable]
	public class Vector3<T>
	{
		public enum Axis { X = 0, Y, Z };

		[JsonIgnore]
		public Func<T, T> Validator;

		private T _x;
		private T _y;
		private T _z;
		public T X
		{
			get { return _x; }
			set { _x = Validator(value); }
		}
		public T Y
		{
			get { return _y; }
			set { _y = Validator(value); }
		}
		public T Z
		{
			get { return _z; }
			set { _z = Validator(value); }
		}

		public Vector3()
		{
			Validator = delegate(T t)
			{
				return t;
			};
		}

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

		public Vector3(T x, T y, T z)
			: this()
		{
			X = x;
			Y = y;
			Z = z;
		}

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

			throw new ArgumentOutOfRangeException("axis");
		}
	}

	public class SVector3 : Vector3<short>
	{
		public SVector3()
		{
			Validator = delegate(short value)
			{
				if (value < -100)
					value = -100;

				if (value > 100)
					value = 100;

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
