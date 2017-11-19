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

		_server.bind(function() {
		    _server.setBroadcast(true);
		});

		getInterfaces(function (list) {
			var _im;
			if (list.length == 0) {
				_im = { ip: "127.0.0.1", mask: "255.255.255.0" };
			}
			else {
				if (config.bcast.iface) {
					log.info("Looking for interface matching: " + JSON.stringify(config.iface));
					var _confBlock = new nm(config.bcast.iface.ip + "/" + config.bcast.iface.mask);
					for (var _i = 0; _i < list.length; _i++) {
						if (_confBlock.contains(list[_i].ip)) {
							_im = list[_i];
						}
					}

					if (_im == null) {
						log.warn("Could not find applicable interface");
					}
				}
				else {
				log.info("No broadcast interface specified");
				}

				if (_im == null) {
					_im = list[0];
					if (_im['ip'] == "127.0.0.1" && list.length > 1)
						_im = list[1];
				}
			}

			var _block = new nm(_im['ip'] + "/" + _im['mask']);
			_addr = _block.broadcast;
			log.info("Addr: " + _addr);

			var _bcast = {
				name: config.name,
				port: config.port.pipe,
				video: config.bcast.video.replace("{IP_ADDRESS}", _im.ip)
			};
			_buffer = new Buffer(JSON.stringify(_bcast) + "\r\n");

			log.info("Final broadcast", _bcast);
			_ready = true;
		});
	};

	function getInterfaces(cb) {
		var _clean = [/^169/];

		var _cmd;
		var _sr;
		switch (process.platform) {
			case "win32":
			case "win64":
				_cmd = "ipconfig";
				_sr = /\bIPv4.*: (\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})\r?\n?.*Subnet Mask.*: (\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})/mg;
				break;

			default:
				_cmd = "ifconfig";
				_sr = /\binet addr:(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}).*?Mask:(\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3})/g;
				break;
		}

		var _isClean = function (ip) {
			for (var _i = 0; _i < _clean.length; _i++) {
				if (_clean[_i].test(ip))
					return false;
			}

			return true;
		};

		exec(_cmd, function (error, stdout) {
			var _list = [];
			var _matches = stdout.match(_sr) || [];
			for (var _i = 0; _i < _matches.length; _i++) {
				var _ip = _matches[_i].replace(_sr, "$1");
				var _mask = _matches[_i].replace(_sr, "$2");

				if (!_isClean(_ip))
					continue;

				_list.push({ ip: _ip, mask: _mask });
			}

			cb(_list);
		});
	};

	this.setPort = function (port) {
		log.info("port: " + port);

		_port = port;
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
