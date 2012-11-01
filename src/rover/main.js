
var config = require("config");

ControlPipe.setPort(config.port.pipe);
BroadcastSender.setPort(config.port.bcast);

ControlPipe.listen();

setInterval(function () { BroadcastSender.broadcast(); }, config.bcast.interval);