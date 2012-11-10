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
	this.name = "CommandNotFoundError";
	this.Command = name;
}

module.exports.ParseFailed = function () {
	this.name = "ParseFailedError";
}

// Thrown when client needs to login.
module.exports.NoLogin = function () {
	this.name = "NoLoginError";
}

// Thrown when invalid credentials have been given.
module.exports.InvalidLogin = function (triesLeft) {
	this.name = "InvalidLoginError";
	this.TriesLeft = triesLeft;
}

module.exports.ConcurrentConnection = function () {
	this.name = "ConcurrentConnectionError";
}