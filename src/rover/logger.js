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

var config = require("./config");

var fs = require("fs");
var path = require("path");
var winston = require("winston");
winston.add(winston.transports.File, {
	// Have to use stream in order to create the file if it does not exist
	stream: fs.createWriteStream(path.resolve(__dirname, "rover.log"), {
		flags: "a",
		mode: 0666
	}),
	timestamp: true,
	json: false,
	handleExceptions: true,
	level: config.debug ? 'info' : 'warn'
});

winston.remove(winston.transports.Console);

if (config.debug) {
	winston.add(winston.transports.Console, {
		timestamp: function () {
			var _now = new Date();
			var _hours = _now.getHours();
			var _minutes = _now.getMinutes();
			var _seconds = _now.getSeconds();
			var _milli = _now.getMilliseconds();

			if (_hours < 10)
				_hours = "0" + _hours;

			if (_minutes < 10)
				_minutes = "0" + _minutes;

			if (_seconds < 10)
				_seconds = "0" + _seconds;

			return _hours.toString() + ":" + _minutes.toString() + ":" + _seconds.toString() + "." + _milli.toString();
		}
	});
	winston.handleExceptions(new winston.transports.Console({ colorize: true, json: true }));
}

module.exports = winston;

module.exports.LabelledLogger = function (label) {
	// Work with a new bare object
	var ret = new Object();

	ret.info = function () {
		var args = arguments;
		args[0] = "[" + label + "] " + args[0];
		winston.info.apply(this, args);
	};

	ret.warn = function () {
		var args = arguments;
		args[0] = "[" + label + "] " + args[0];
		winston.warn.apply(this, args);
	};

	ret.error = function () {
		var args = arguments;
		args[0] = "[" + label + "] " + args[0];
		winston.error.apply(this, args);
	};

	return ret;
};