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

var log = new require("./logger").LabelledLogger("states");
var config = require("./config");
var pins = require("./pins");

if (config.simulator)
	require("./bone");
else
	require("bonescript");

// Hacky. It's just a band-aid until #49 is implemented
pinMode(bone[pins.motor.forward_reverse.speed], OUTPUT);
pinMode(bone[pins.motor.forward_reverse.dir], OUTPUT);
pinMode(bone[pins.motor.turn.speed], OUTPUT);
pinMode(bone[pins.motor.turn.dir], OUTPUT);
pinMode(bone[pins.light.head.pin], OUTPUT);

module.exports.TurnState = function (data) {
	log.info("TurnState", data);

	var motor = pins.motor.turn;

	if (data.Vector.Y == 0) {
		analogWrite(bone[motor.speed], 0);
	}
	else if (data.Vector.Y > 0) { // right turn
		digitalWrite(bone[motor.dir], 1);
		analogWrite(bone[motor.speed], 1.0);
		// analogWrite(bone[motor.speed], data.Vector.Y);
	}
	else {
		digitalWrite(bone[motor.dir], 0);
		analogWrite(bone[motor.speed], 1.0);
		// analogWrite(bone[motor.speed], data.Vector.Y);
	}
};

module.exports.MoveState = function (data) {
	log.info("MoveState", data);

	var motor = pins.motor.forward_reverse;

	if (data.Vector.Z == 0) {
		analogWrite(bone[motor.speed], 0);
	}
	else if (data.Vector.Z > 0) {
		digitalWrite(bone[motor.dir], 0);
		analogWrite(bone[motor.speed], data.Vector.Z * 0.004 + 0.6);
	}
	else {
		digitalWrite(bone[motor.dir], 1);
		analogWrite(bone[motor.speed], data.Vector.Z * (-0.004) + 0.6);
	}
};

module.exports.LightState = function (data) {
	log.info("LightState", data);

	if (!pins.light.hasOwnProperty(data.Id))
		throw new Error("Light does not exist.");

	var light = pins.light[data.Id];

	if (light.pwm)
		analogWrite(bone[light.pin], data.Level * 0.01);
	else
		digitalWrite(bone[light.pin], data.Level > 0);
}