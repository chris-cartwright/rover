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

// Hacky. It's just a band-aid until #49 is implemented
pinMode(pins.motor.forward_reverse.speed, OUTPUT);
pinMode(pins.motor.forward_reverse.dir, OUTPUT);
pinMode(pins.motor.turn.speed, OUTPUT);
pinMode(pins.motor.turn.dir, OUTPUT);

module.exports.TurnState = function (data) {
    log.info("TurnState", data);

    var motor = pins.motor.turn;
    if (data.Vector.X > 0) { // right turn
        digitalWrite(bone[motor.dir], 1);
        analogWrite(bone[motor.speed], 1.0);
       // analogWrite(bone[motor.speed], data.Vector.X);
    }
    else {
        digitalWrite(bone[motor.dir], 0);
        analogWrite(bone[motor.speed], 1.0);
        // analogWrite(bone[motor.speed], data.Vector.X);
    } 
};

//module.exports.ForwardMoveState = function (data) {
//    log.info("ForwardMoveState", data);

//    data.Vector.X = data.Vector.X * 0.004 + 0.6;  // should produce results between 0.6 and 1.0 from input 0 to 100
//    // should to be abs(data.Speed) as non-directional speed?... as we are not using sign for direction

//    var motor = pins.motor.forward_reverse;
//    digitalWrite(bone[motor.dir], 0);
//    analogWrite(bone[motor.speed], data.Vector.X);
//};

//module.exports.BackwardMoveState = function (data) {
//    log.info("BackwardMoveState", data);

//    data.Vector.X = data.Vector.X * 0.004 + 0.6; // should to be abs(data.Speed) as non-directional speed?... as we are not using sign for direction
//    var motor = pins.motor.forward_reverse;
//    digitalWrite(bone[motor.dir], 1);
//    analogWrite(bone[motor.speed], data.Vector.X);
//};

module.exports.MoveState = function (data) {
    log.info("MoveState", data);

   // data.Vector.X = data.Vector.X * 0.004 + 0.6; // should to be abs(data.Speed) as non-directional speed?... as we are not using sign for direction
    var motor = pins.motor.forward_reverse;

    if (data.Vector.X > 0) {
        digitalWrite(bone[motor.dir], 0);
        analogWrite(bone[motor.speed], data.Vector.X * 0.004 + 0.6);
    }
    else {
        digitalWrite(bone[motor.dir], 1);
        analogWrite(bone[motor.speed], data.Vector.X * (-0.004) + 0.6);
    }
};

//module.exports.LeftTurnState = function (data) {
//    log.info("LeftTurnState", data);

//    var motor = pins.motor.turn;
//    digitalWrite(bone[motor.dir], 0);
//    analogWrite(bone[motor.speed], 1.0);
//};

//module.exports.RightTurnState = function (data) {
//    log.info("RightTurnState", data);

//    var motor = pins.motor.turn;
//    digitalWrite(bone[motor.dir], 1);
//    analogWrite(bone[motor.speed], 1.0);
//};