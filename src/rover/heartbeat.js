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

var log = new require("./logger").LabelledLogger("heartbeat");
var config = require("./config");

var _timer = null;
var _last = 0;
var _pipe = null;

function epoch() {
	return (new Date()).getTime();
}

function _beat() {
	if (_last != 0 && epoch() > config.heartbeat * 2 + _last)
		_pipe.close();
	else
		_pipe.send({ name: "Heartbeat" });
}

module.exports.start = function () {
	_pipe = require("./control_pipe");
	setInterval(_beat, config.heartbeat);
	_last = epoch();
};

module.exports.stop = function () {
	clearInterval(_timer);
	_timer = null;
};

module.exports.beat = function () {
	_last = epoch();
};