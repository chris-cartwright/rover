/*
Copyright (C) 2013 Christopher Cartwright
    
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

var log = new require("./logger").LabelledLogger("local");
var config = require("./config");
var pins = require("./pins");
var arduino = require("./arduino");
var os = require("os");

var bone = require.main.exports.bone;

arduino.connect();

var unlock = null;
var orig;

bone.attachInterrupt(pins.button.show_ip, function (x) {
	log.info("show_ip change", x.value);
	
	if (x.value == bone.HIGH) {
		var ifaces = os.networkInterfaces();

		orig = arduino.lcd.isOn();
		arduino.lcd.on();
		arduino.lcd.clear();
		for (var i = 0; i < config.arduino.interfaces.length && i < 2; i++) {
			var iface = config.arduino.interfaces[i];
			if (!ifaces.hasOwnProperty(iface)) {
				continue;
			}
			
			for (var j = 0; j < ifaces[iface].length; j++) {
				if (ifaces[iface][j].family == "IPv4") {
					arduino.lcd.write(ifaces[iface][j].address);
					break;
				}
			}
		}
		
		unlock = arduino.lcd.lock();
	} else if (typeof (unlock) == "function") {
		unlock();
		unlock = null;
		arduino.lcd.clear();
		if (orig == false) {
			arduino.lcd.off();
		}
	}

	return this;
}, bone.CHANGE);

arduino.enable();