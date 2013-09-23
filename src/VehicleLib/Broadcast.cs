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

using System.Net;

namespace VehicleLib
{
	public class Broadcast
	{
		public IPEndPoint Endpoint { get; private set; }
		public string Name { get; private set; }
		public string VideoFeed { get; private set; }

		public Broadcast(IPEndPoint endpoint, string name, string videoFeed)
		{
			Endpoint = endpoint;
			Name = name;
			VideoFeed = videoFeed;
		}
	}
}
