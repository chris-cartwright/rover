
var log = new require("./logger").LabelledLogger("main");
var fs = require("fs");
var serial = require("serialport");

module.exports.uarts = {
	1: {
		tty: "/dev/ttyO1",
		rxd: {
			file: "uart1_rxd",
			mode: 0
		},
		txd: {
			file: "uart1_txd",
			mode: 0
		}
	},
	2: {
		tty: "/dev/ttyO2",
		rxd: {
			file: "spi0_sclk",
			mode: 1
		},
		txd: {
			file: "spi0_d0",
			mode: 1
		}
	},
	4: {
		tty: "/dev/ttyO4",
		rxd: {
			file: "gpmc_wait0",
			mode: 6
		},
		txd: {
			file: "gpmc_wpn",
			mode: 6
		}
	},
	5: {
		tty: "/dev/ttyO5",
		rxd: {
			file: "lcd_data9",
			mode: 5
		},
		txd: {
			file: "lcd_data8",
			mode: 5
		}
	}
};

module.exports.connect = function(uart) {
	log.info("Connect serial: " + uart.tty);

	var fd;

	fd = fs.openSync("/sys/kernel/debug/omap_mux/" + uart.rxd.file, "w");
	fs.writeSync(fd, uart.rxd.mode, null);
	fs.closeSync(fd);

	fd = fs.openSync("/sys/kernel/debug/omap_mux/" + uart.txd.file, "w");
	fs.writeSync(fd, uart.txd.mode, null);
	fs.closeSync(fd);

	return new serial.SerialPort(uart.tty);
};
