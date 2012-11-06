
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