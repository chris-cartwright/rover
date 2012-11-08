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

var dgram = require("dgram");
var config = require("./config");
var log = require("./logger");

var BroadcastSender = new function () {
	var _port = 0;
	var _buffer = null;
	var _server = dgram.createSocket("udp4");
	var _setup = false;

	function setup() {
		_server.bind(_port);
		_server.setBroadcast(true);
		_setup = true;
	};

	this.setPort = function (port) {
		_port = port;
		_buffer = new Buffer(JSON.stringify({ name: config.name, port: config.port.pipe }));
	};

	this.broadcast = function () {
		if (!_setup)
			setup();

		_server.send(_buffer, 0, _buffer.length, _port, config.bcast.addr);
	};
};

module.exports = BroadcastSender;