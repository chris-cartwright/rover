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

This code is based on bonescript by Jason Kridner.
Copyright (c) 2011 Jason Kridner <jdk@ti.com>
<https://github.com/jadonk/bonescript>
*/

var net = require("net");
var log = new require("./logger").LabelledLogger("bone");
var pins = require("./pins");

var OUTPUT = exports.OUTPUT = "output";
var INPUT = exports.INPUT = "input";
var HIGH = exports.HIGH = 1;
var LOW = exports.LOW = 0;
var CHANGE = exports.CHANGE = "change";
var RISING = exports.RISING = "rising";
var FALLING = exports.FALLING = "falling";

var server = net.createServer();
var clients = [];
var reads = [];

var activePins = [];

server.on("connection", function (socket) {
	log.info("Sim connection: " + socket.remoteAddress);
	clients.push(socket);

	var _proto = new require("json-line-protocol").JsonLineProtocol();
	_proto.on("value", function (obj) {
		if (obj.cmd == "pinValue")
			bone[obj.pin].value = obj.value;
		else if (obj.cmd == "interrupt") {
			for (var _i = 0; _i < bone[obj.pin].handlers[obj.mode].length; _i++) {
				bone[obj.pin].handlers[obj.mode][_i]({ pin: obj.pin, value: obj.value });
			}
		}
		else
			log.warn("Unknown command: " + obj.cmd);
	});

	socket.on("end", function () {
		clients.splice(clients.indexOf(socket), 1);
		log.info("Sim left: " + socket.remoteAddress);
	});

	socket.on("data", function (data) {
		_proto.feed(data);
	});

	clients.write(JSON.stringify({ cmd: "pins", data: pins }) + "\r\n");
});

server.on("listening", function () {
	log.info("Sim server listening: " + JSON.stringify(server.address()));
});

function bcast(data) {
	data = JSON.stringify(data);
	log.info("Bcast: " + data);

	data += "\r\n";

	clients.forEach(function (client) {
		client.write(data);
	});
}

function validPin(pin) {
	if (!bone.hasOwnProperty(pin))
		log.error("Invalid pin: " + pin);
}

function validMode(mode) {
	if (mode != OUTPUT && mode != INPUT)
		log.error("Invalid mode: " + mode);
}

function validBool(bool) {
	if (bool != HIGH && bool != LOW)
		log.error("Invalid bool: " + bool);
}

function validAnalog(value) {
	if (value < 0 || value > 1)
		log.error("Invalid analog: " + value);
}

function activePin(pin) {
	if (!activePins.hasOwnProperty(pin))
		log.error("Inactive pin used: " + pin);
}

var bone = exports.bone = {
	P8_1: { name: "P8_1", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_2: { name: "P8_2", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_3: { name: "P8_3", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_4: { name: "P8_4", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_5: { name: "P8_5", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_6: { name: "P8_6", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_7: { name: "P8_7", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_8: { name: "P8_8", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_9: { name: "P8_9", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_10: { name: "P8_10", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_11: { name: "P8_11", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_12: { name: "P8_12", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_13: { name: "P8_13", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_14: { name: "P8_14", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_15: { name: "P8_15", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_16: { name: "P8_16", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_17: { name: "P8_17", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_18: { name: "P8_18", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_19: { name: "P8_19", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_20: { name: "P8_20", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_21: { name: "P8_21", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_22: { name: "P8_22", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_23: { name: "P8_23", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_24: { name: "P8_24", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_25: { name: "P8_25", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_26: { name: "P8_26", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_27: { name: "P8_27", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_28: { name: "P8_28", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_29: { name: "P8_29", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_30: { name: "P8_30", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_31: { name: "P8_31", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_32: { name: "P8_32", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_33: { name: "P8_33", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_34: { name: "P8_34", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_35: { name: "P8_35", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_36: { name: "P8_36", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_37: { name: "P8_37", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_38: { name: "P8_38", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_39: { name: "P8_39", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_40: { name: "P8_40", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_41: { name: "P8_41", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_42: { name: "P8_42", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_43: { name: "P8_43", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_44: { name: "P8_44", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_45: { name: "P8_45", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P8_46: { name: "P8_46", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },

	P9_1: { name: "P9_1", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_2: { name: "P9_2", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_3: { name: "P9_3", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_4: { name: "P9_4", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_5: { name: "P9_5", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_6: { name: "P9_6", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_7: { name: "P9_7", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_8: { name: "P9_8", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_9: { name: "P9_9", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_10: { name: "P9_10", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_11: { name: "P9_11", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_12: { name: "P9_12", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_13: { name: "P9_13", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_14: { name: "P9_14", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_15: { name: "P9_15", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_16: { name: "P9_16", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_17: { name: "P9_17", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_18: { name: "P9_18", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_19: { name: "P9_19", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_20: { name: "P9_20", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_21: { name: "P9_21", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_22: { name: "P9_22", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_23: { name: "P9_23", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_24: { name: "P9_24", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_25: { name: "P9_25", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_26: { name: "P9_26", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_27: { name: "P9_27", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_28: { name: "P9_28", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_29: { name: "P9_29", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_30: { name: "P9_30", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_31: { name: "P9_31", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_32: { name: "P9_32", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_33: { name: "P9_33", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_34: { name: "P9_34", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_35: { name: "P9_35", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_36: { name: "P9_36", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_37: { name: "P9_37", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_38: { name: "P9_38", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_39: { name: "P9_39", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_40: { name: "P9_40", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_41: { name: "P9_41", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_42: { name: "P9_42", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_43: { name: "P9_43", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_44: { name: "P9_44", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_45: { name: "P9_45", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } },
	P9_46: { name: "P9_46", value: LOW, handlers: { "change": [], "rising": [], "falling": [] } }
};

exports.pinMode = function (pin, mode) {
	validPin(pin);
	validMode(mode);

	activePins.push(pin);

	bcast({ cmd: "pinMode", pin: pin, mode: mode });
};

exports.attachInterrupt = function (pin, handler, mode) {
	validPin(pin);
	if (typeof (handler) != "function")
		log.error("Handler is not a function: " + String(handler));

	if (mode != CHANGE && mode != RISING && mode != FALLING)
		log.error("Invalid mode: " + mode);

	bcast({ cmd: "attachInterrupt", pin: pin, mode: mode });
	bone[pin].handlers[mode].push(handler);
};

exports.digitalWrite = function (pin, value) {
	validPin(pin);
	validBool(value);
	activePin(pin);

	bcast({ cmd: "digitalWrite", pin: pin, value: value });
	bone[pin].value = value;
};

exports.digitalRead = function (pin) {
	validPin(pin);
	activePin(pin);

	bcast({ cmd: "digitalRead", pin: pin });
	return bone[pin].value;
};

exports.analogWrite = function (pin, value) {
	validPin(pin);
	validAnalog(value);
	activePin(pin);

	bcast({ cmd: "analogWrite", pin: pin, value: value });
	bone[pin].value = value;
};

exports.analogRead = function (pin) {
	validPin(pin);
	activePin(pin);

	bcast({ cmd: "analogRead", pin: pin });
	return bone[pin].value;
};

module.exports.listen = function () {
	server.listen(0);
};