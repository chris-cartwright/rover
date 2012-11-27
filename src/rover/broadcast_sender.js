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
var log = new require("./logger").LabelledLogger("bcast_sender");
var nm = require("netmask").Netmask;
var exec = require("child_process").exec;

var BroadcastSender = new function () {
	var _port = 0;
	var _buffer = null;
	var _server = dgram.createSocket("udp4");
	var _ready = false;
	var _addr = "";

	function setup() {
		log.info("setup");

		_server.bind(0);
		_server.setBroadcast(true);
		_setup = true;

		getInterfaces(function (list) {
			var im = null;
			if (list.length == 0) {
				im = { ip: "127.0.0.1", mask: "255.255.255.0" };
			}
			else {
				im = list[0];
				if (im['ip'] == "127.0.0.1" && list.length > 1)
					im = list[1];
			}

			var block = new nm(im['ip'] + "/" + im['mask']);
			_addr = block.broadcast;
			log.info("Addr: " + _addr);
			_ready = true;
		});
	};

	function getInterfaces(cb) {
		var clean = [/^169/];

		var cmd = "";
		var sr = "";
		switch (process.platform) {
			case "win32":
			case "win64":
				cmd = "ipconfig";
				sr = /\bIPv4.*: (\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})\r?\n?.*Subnet Mask.*: (\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})/mg;
				break;

			default:
				cmd = "ifconfig";
				sr = /\binet addr:(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}).*?Mask:(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})/g;
				break;
		}

		var isClean = function (ip) {
			for (var i = 0; i < clean.length; i++) {
				if (clean[i].test(ip))
					return false;
			}

			return true;
		};

		exec(cmd, function (error, stdout, stderr) {
			var list = [];
			var matches = stdout.match(sr) || [];
			for (var i = 0; i < matches.length; i++) {
				var ip = matches[i].replace(sr, "$1");
				var mask = matches[i].replace(sr, "$2");

				if (!isClean(ip))
					continue;

				list.push({ ip: ip, mask: mask });
			}

			cb(list);
		});
	};

	this.setPort = function (port) {
		log.info("port: " + port);

		_port = port;
		_buffer = new Buffer(JSON.stringify({ name: config.name, port: config.port.pipe }) + "\r\n");
		setup();
	};

	this.broadcast = function () {
		log.info("bcast");

		if (!_ready)
			return;

		_server.send(_buffer, 0, _buffer.length, _port, _addr);
	};
};

module.exports = BroadcastSender;