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

var pins = {
	motor: {
		forward_reverse: {
			speed: P9_14,
			dir: P9_25
		},
		turn: {
			speed: P9_16,
			dir: P9_27
		}
	}
	/*
	Hookups that exist in hardware, but haven't been hooked up yet
	light: {
		head: P9_,
		larson_1: P9_,
		larson_2: P9_,
		larson_3: P9_
	},
	rgb_light: {
		power: {
			r: P9_,
			g: P9_,
			b: P9_
		}
	},
	sensor: {
		light_left: P9_,
		light_right: P9_
	}
	*/
};

module.exports = pins;