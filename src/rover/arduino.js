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

var log = new require("./logger").LabelledLogger("arduino");
var config = require("./config");
var serial = require("serialport");
var pins = require("./pins");

var bone = require.main.exports.bone;

var connected = false;
var locked = false;
var callbacks = [];
var isOn = true;
var enabled = false;

var ser = new serial.SerialPort("/dev/ttyO" + config.arduino.uart, {
	baudrate: config.arduino.baud,
	parser: serial.parsers.readline("\r")
});

function onData(data) {
	log.debug("data received", data);
	callbacks.shift()(data);
}

module.exports.connect = function () {
	ser.open(function () {
		log.info("Port opened");
		connected = true;

		ser.on("data", onData);

		ser.on("error", function (data) {
			log.info("Port error");
			throw data;
		});

		ser.on("close", function () {
			log.info("Port closed");
			connected = false;
		});
	});
};

module.exports.enabled = function() {
	return enabled;
};

module.exports.enable = function() {
	bone.digitalWrite(pins.power.arduino, bone.HIGH);
	enabled = true;
};

module.exports.disable = function() {
	bone.digitalWrite(pins.power.arduino, bone.LOW);
	enabled = false;
};

module.exports.analogRead = function (port, callback) {
	log.info("analogRead", port);

	port = parseInt(port);
	if (port < 1 || port > 21) {
		throw new Error("port must be between 1 and 21");
	}

	callbacks.push(callback);

	ser.write(0x81);
	ser.write(port);
};

module.exports.connected = function () {
	return connected;
};

var lcd = {
	write: function (str) {
		log.info("lcd.writeString", str);
		ser.write(String(str));
	},
	clear: function () {
		log.info("lcd.clear");
		ser.write([0xFE, 0x01]);
	},
	pos: function (row, column) {
		log.info("lcd.pos", row, column);
		if (row < 0 || row > 1) {
			throw new Error("row must be between 0 and 1");
		}

		if (column < 0 || column > 15) {
			throw new Error("column must be between 0 and 15");
		}

		if (row == 1) {
			column += 16;
		}

		ser.write([0xFE, 0x80, column]);
	},
	on: function () {
		log.info("lcd.on");
		ser.write([0xFE, 0x0C]);
		isOn = true;
	},
	off: function () {
		log.info("lcd.off");
		ser.write([0xFE, 0x08]);
		isOn = false;
	},
	isOn: function() {
		return isOn;
	},
	lock: function () {
		log.info("lcd.lock");

		// pattern proven under proofs/lock-unlock.js
		if (locked) {
			throw new Error("Already locked");
		}

		locked = true;
		return function () {
			var _used = false;

			// Double closure to make sure the user
			// can only release once
			return function () {
				log.info("unlock");

				if (_used) {
					log.info("already used");
					return;
				}

				_used = true;
				locked = false;
			};
		}();
	}
};

module.exports.lcd = {};

for (var prop in lcd) {
	if (!lcd.hasOwnProperty(prop)) {
		continue;
	}

	(function () {
		var _p = prop;
		module.exports.lcd[_p] = function () {
			if (locked) {
				log.info("locked");
				return null;
			}

			return lcd[_p].apply(this, arguments);
		};
	})();
}