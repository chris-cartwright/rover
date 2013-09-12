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

var jsonLineProtocol = require("json-line-protocol");
var net = require("net");
var config = require("./config");
var states = require("./states");
var queries = require("./queries");
var err = require("./errors");
var log = new require("./logger").LabelledLogger("control_pipe");
var events = require("./events");

var ControlPipe = new function () {
	var _self = this;

	var _server = null;
	var _client = null;
	var _port = 0;
	var _validated = false;
	var _tries = 0;
	var _listening = false;
	var _proto = new jsonLineProtocol.JsonLineProtocol();

	_proto.on("value", onCommand);
	_server = net.createServer();
	_server.on("close", onDisconnect);
	_server.on("error", onError);
	_server.on("listening", onListening);
	_server.on("connection", onConnect);

	function onConnect(socket) {
		log.info("New connection: " + JSON.stringify(socket.address()));

		if (_client != null) {
			var e = new err.ConcurrentConnection();
			_self.send(e, null, socket);
			return;
		}

		_client = socket;
		_client.on("data", onData);
		_client.on("error", onClientError);
		_client.on("close", onDisconnect);
		_validated = false;
		_tries = 0;
	}

	function onData(data) {
		log.info("Data received: " + data);
		try {
			_proto.feed(data);
		}
		catch (e) {
			// This also catches error originating in onCommand
			if (e.name == "SyntaxError")
				_self.send(new err.ParseFailed());
			else
				_self.send(new err.Unknown(e.message));
		}
	}

	function onCommand(obj) {
		log.info("Command received: " + obj.cmd);

		if (!_validated) {
			if (obj.cmd != "Login") {
				_self.send(new err.NoLogin());
				return;
			}

			if (obj.data.Password != config.passwd) {
				_tries++;
				_self.send(new err.InvalidLogin(3 - _tries));

				if (_tries >= 3) {
					_client.end();
					_client = null;
				}

				return;
			}

			_validated = true;
			_self.send(new events.LoginSuccess());
			return;
		}

		if (obj.cmd.indexOf("State") != -1) {
			if (!states.hasOwnProperty(obj.cmd)) {
				_self.send(new err.CommandNotFound(obj.cmd));
				return;
			}

			try {
				states[obj.cmd](obj.data);
			}
			catch (e) {
				_self.send(new err.CommandFailed(obj.cmd, e));
			}
		}
		else if (obj.cmd.indexOf("Query") != -1) {
			if (!queries.hasOwnProperty(obj.cmd)) {
				_self.send(new err.CommandNotFound(obj.cmd));
				return;
			}

			try {
				var ret = queries[obj.cmd](obj.data.Sensor);
				_self.send(ret, obj.id);
			}
			catch (e) {
				_self.send(new err.CommandFailed(obj.cmd, e));
			}
		}
		else
			_self.send(new err.CommandNotFound(obj.cmd));
	}

	function onDisconnect() {
		log.info("Disconnected.");

		_client = null;
		_validated = false;
		_tries = 0;
	}

	function onError(e) {
		if (e.code == "EADDRINUSE")
			throw new Error("Address already in use.");
	}

	function onClientError(e) {
		throw e;
	}

	function onListening() {
		_listening = true;
	}

	this.setPort = function (port) {
		_port = port;
	};

	this.listen = function () {
		if (_port == null)
			throw new Error("Port does not contain a value.");

		if (_listening)
			throw new Error("Already listening for connections.");

		_server.listen(_port);
	};

	this.send = function (obj, id, socket) {
		log.info("Data sent: ", JSON.stringify({ object: obj, id: id }));

		var name = obj.name;
		delete obj.name;
		var d = {
			cmd: name,
			data: obj
		};

		if (typeof (id) != "undefined" && id != null)
			d.id = id;

		var msg = JSON.stringify(d) + "\r\n";
		if (typeof (socket) != "undefined")
			socket.write(msg);
		else
			_client.write(msg);
	};

	this.isListening = function () {
		return _listening;
	};
};

module.exports = ControlPipe;