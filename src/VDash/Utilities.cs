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
