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

var bone;
if (config.simulator)
	bone = require("./bone");
else
	bone = require("bonescript");

// Hacky. It's just a band-aid until #49 is implemented
// Don't set pinMode on PWM pins
bone.pinMode(pins.motor.forward_reverse.dir, bone.OUTPUT);
bone.pinMode(pins.motor.turn.dir, bone.OUTPUT);
bone.pinMode(pins.power.arduino, bone.OUTPUT);
bone.pinMode(pins.button.show_ip, bone.INPUT);

module.exports.TurnState = function (data) {
	log.info("TurnState", data);

	var motor = pins.motor.turn;

	if (data.Vector.Y == 0) {
		bone.analogWrite(motor.speed, 0);
	}
	else if (data.Vector.Y > 0) { // right turn
		bone.digitalWrite(motor.dir, 1);
		bone.analogWrite(motor.speed, 1.0);
		// analogWrite(bone[motor.speed], data.Vector.Y);
	}
	else {
		bone.digitalWrite(motor.dir, 0);
		bone.analogWrite(motor.speed, 1.0);
		// analogWrite(bone[motor.speed], data.Vector.Y);
	}
};

module.exports.MoveState = function (data) {
	log.info("MoveState", data);

	var motor = pins.motor.forward_reverse;

	if (data.Vector.Z == 0) {
		bone.analogWrite(motor.speed, 0);
	}
	else if (data.Vector.Z > 0) {
		bone.digitalWrite(motor.dir, 0);
		bone.analogWrite(motor.speed, data.Vector.Z * 0.004 + 0.6);
	}
	else {
		bone.digitalWrite(motor.dir, 1);
		bone.analogWrite(motor.speed, data.Vector.Z * (-0.004) + 0.6);
	}
};

module.exports.LightState = function (data) {
	log.info("LightState", data);

	if (!pins.light.hasOwnProperty(data.Id))
		throw new Error("Light does not exist.");

	var light = pins.light[data.Id];

	if (light.pwm)
		bone.analogWrite(light.pin, data.Level * 0.01);
	else
		bone.digitalWrite(light.pin, data.Level > 0);
};
