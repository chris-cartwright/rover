
var config = require("./config");
var log = require("./logger");

log.info("Loading local libraries...");
var ControlPipe = require("./control_pipe");
//var BroadcastSender = require("./broadcast_sender");
log.info("Loaded.");

ControlPipe.setPort(config.port.pipe);
//BroadcastSender.setPort(config.port.bcast);
log.info("Set ports.");

ControlPipe.listen();

log.info("Listening for connections...");

//setInterval(function () { BroadcastSender.broadcast(); }, config.bcast.interval);