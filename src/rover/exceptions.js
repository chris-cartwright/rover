
/*
No message should be included in any exception.
	The server can provide a meaningful message based on the exception's name.
	This allows for internationalization if needed as well as transfering less
	data on what could be a limited pipe.

Each message should have a 'name' property.
	JavaScript is a typeless system. A 'class' name cannot be deduced from the
	object itself. The name is required for proper deserialization in VDash.
*/

module.exports.CommandNotFound = function (name) {
	this.name = "CommandNotFoundException";
	this.Command = name;
}

module.exports.ParseFailed = function () {
	this.name = "ParseFailedException";
}

// Thrown when client needs to login.
module.exports.NoLogin = function () {
	this.name = "NoLoginException";
}

// Thrown when invalid credentials have been given.
module.exports.InvalidLogin = function (triesLeft) {
	this.name = "InvalidLoginException";
	this.TriesLeft = triesLeft;
}

module.exports.ConcurrentConnection = function () {
	this.name = "ConcurrentConnectionException";
}