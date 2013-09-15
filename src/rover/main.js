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

var config = require("./config");
var log = new require("./logger").LabelledLogger("main");

// module.exports.bone should be set up before other includes try to use it
if (config.simulator) {
	log.info("Loading debug simulator...");
	module.exports.bone = require("./bone");
	module.exports.bone.listen();
}
else
	module.exports.bone = require("bonescript");

log.info("Loading local libraries...");
var ControlPipe = require("./control_pipe");
var BroadcastSender = require("./broadcast_sender");
log.info("Loaded.");

ControlPipe.setPort(config.port.pipe);
BroadcastSender.setPort(config.port.bcast);
log.info("Set ports.");

log.info("Starting local services...");
require("./local");

ControlPipe.listen();

log.info("Listening for connections...");

setInterval(function () { BroadcastSender.broadcast(); }, config.bcast.interval);
