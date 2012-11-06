
var jsonLineProtocol = require("json-line-protocol");
var net = require("net");
var config = require("./config");
var states = require("./states");
var queries = require("./queries");
var ex = require("./exceptions");
var log = require("./logger");

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
			var e = new ex.ConcurrentConnection();

			var name = e.name;
			delete e.name;
			var d = {
				cmd: name,
				data: e
			};

			socket.write(JSON.stringify(d), function () { socket.end(); });
			return;
		}

		_client = socket;
		_client.on("data", onData);
		_client.on("error", onClientError);
		_validated = false;
		_tries = 0;
	};

	function onData(data) {
		log.info("Data received: " + data);
		try {
			_proto.feed(data);
		} catch (e) {
			// This also catches error originating in onCommand
			if (e.name == "SyntaxError")
				_self.sendException(new ex.ParseFailed());
			else
				throw e;
		}
	};

	function onCommand(obj) {
		log.info("Command received: " + obj.cmd);

		if (!_validated) {
			if (obj.cmd != "Login") {
				_self.sendException(new ex.NoLogin());
				return;
			}

			if (obj.data != config.passwd) {
				_tries++;
				_self.sendException(new ex.InvalidLogin(3 - _tries));

				if (_tries >= 3) {
					_client.end();
					_client = null;
				}

				return;
			}

			_validated = true;
			return;
		}

		if (obj.cmd.indexOf("State") != -1)
			state[obj.cmd](obj.data);
		else if (obj.cmd.indexOf("Query") != -1) {
			var ret = queries[obj.cmd](obj.data.id);
			var name = ret.name;
			delete ret.name;
			var d = {
				cmd: name,
				data: ret,
				id: obj.id
			};

			_client.write(JSON.stringify(d));
		}
		else
			_self.sendException(new ex.CommandNotFound(obj.cmd));
	};

	function onDisconnect() {
		log.info("Disconnected.");

		_client = null;
		_validated = false;
		_tries = 0;
	};

	function onError(e) {
		if (e.code == "EADDRINUSE")
			throw new Error("Address already in use.");
	};

	function onClientError(e) {
		throw e;
	};

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

	/**
	* Handles one-off sensor information. Eg: a button press.
	*/
	this.sendSensorInfo = function (sensorInfo) {
		var name = sensorInfo.name;
		delete sensorInfo.name;
		var d = {
			cmd: name,
			data: sensorInfo
		};

		_client.write(JSON.stringify(d));
	};

	this.sendException = function (ex) {
		var name = ex.name;
		delete ex.name;
		var d = {
			cmd: name,
			data: ex
		};

		_client.write(JSON.stringify(d));
	};

	this.isListening = function () {
		return _listening;
	};
}

module.exports = ControlPipe;