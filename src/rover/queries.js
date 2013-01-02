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

var log = new require("./logger").LabelledLogger("queries");
var config = require("./config");
var pins = require("./pins");
var sensors = require("./sensors");

// Calculate Vin from Vout, R1, R2 in a voltage divider
function vdin(raw, r1, r2) {
	// Vout = Vin * (R2 / (R1 + R2))
	return raw * (r1 + r2) / r2;
}

module.exports.VoltageQuery = function (sensor) {
	var min = 0;
	var max = 0;
	var current = 0;

	switch (sensor) {
		case "battery":
			current = analogRead(bone[pins.sensor.battery]);
			current = vdin(current, 4760, 980);
			// Rechargeables used measured 1.4v after full charge
			max = 8.4;
			min = 6.0;
			break;
	}

	return new sensors.Voltage(min, max, current);
}