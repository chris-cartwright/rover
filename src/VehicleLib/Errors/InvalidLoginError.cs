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

namespace VehicleLib.Errors
{
	/// <summary>
	/// Returned from vehicle when a login attempt fails.
	/// </summary>
	[Serializable]
	public class InvalidLoginError : Error
	{
		/// <summary>
		/// Number of tried left before vehicle drops connection.
		/// </summary>
		public short TriesLeft { get; set; }

		public InvalidLoginError() : base("Invalid credentials.") { }

		public override string ToString()
		{
			return String.Format("{0} [{1}]", Message, TriesLeft);
		}
	}
}
