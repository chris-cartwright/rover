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

using PostSharp.Extensibility;

namespace Aspects
{
	/// <summary>
	/// Extensions methods and global variables
	/// </summary>
	internal static class Statics
	{
		/// <summary>
		/// Used to mark messages with VDash and load them from resx
		/// </summary>
		public static readonly MessageSource Message = new MessageSource("VDash", new MessageDispenser());
	}
}
