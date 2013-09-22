/*
Copyright (C) 2013 Christopher Cartwright
    
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

namespace VDash
{
	public static class Utilities
	{
		/// <summary>
		/// Determines if the difference between two floating point values is within a tolerance
		/// </summary>
		/// <remarks>
		/// Floating point values cannot be accurately represented in binary, so there is some difference
		/// between the actual value and the stored value. This difference should be taken into account
		/// for most comparisons.
		/// </remarks>
		/// <param name="lhs">First value to compare</param>
		/// <param name="rhs">Second value to compare</param>
		/// <param name="epsilon">Acceptable tolerance</param>
		/// <returns>Difference is within tolerance</returns>
		public static bool Within(this float lhs, float rhs, float epsilon = Single.Epsilon)
		{
			return lhs - rhs < epsilon;
		}

		/// <summary>
		/// Determines if the difference between two floating point values is within a tolerance
		/// </summary>
		/// <remarks>
		/// Floating point values cannot be accurately represented in binary, so there is some difference
		/// between the actual value and the stored value. This difference should be taken into account
		/// for most comparisons.
		/// </remarks>
		/// <param name="lhs">First value to compare</param>
		/// <param name="rhs">Second value to compare</param>
		/// <param name="epsilon">Acceptable tolerance</param>
		/// <returns>Difference is within tolerance</returns>
		public static bool Within(this double lhs, double rhs, double epsilon = Double.Epsilon)
		{
			return lhs - rhs < epsilon;
		}
	}
}
