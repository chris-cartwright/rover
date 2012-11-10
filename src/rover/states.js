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

if (config.simulator) {
	var b = require("./bone");
	b.listen();
}
else
	require("bonescript");

module.exports.ForwardMoveState = function (data) {
	log.info("ForwardMoveState", data);

	var motor = pins.motor.forward_reverse;
	digitalWrite(bone[motor.dir], 1);
	analogWrite(bone[motor.speed], data.Speed);
};

module.exports.BackwardMoveState = function (data) {
	log.info("BackwardMoveState", data);

	var motor = pins.motor.forward_reverse;
	digitalWrite(bone[motor.dir], 0);
	analogWrite(bone[motor.speed], data.Speed);
};

module.exports.LeftTurnState = function (data) {
	log.info("LeftTurnState", data);

	var motor = pins.motor.turn;
	digitalWrite(bone[motor.dir], 0);
	analogWrite(bone[motor.speed], 1.0);
};

module.exports.RightTurnState = function (data) {
	log.info("RightTurnState", data);

	var motor = pins.motor.turn;
	digitalWrite(bone[motor.dir], 1);
	analogWrite(bone[motor.speed], 1.0);
};