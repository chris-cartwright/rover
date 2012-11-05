
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

if(config.debug)
	winston.handleExceptions(new winston.transports.Console({ colorize: true, json: true }));
else
	winston.remove(winston.transports.Console);

module.exports = winston;