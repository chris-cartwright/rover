
var pins = {
	motor: {
		forward_reverse: {
			speed: P9_14,
			dir: P9_25
		},
		turn: {
			speed: P9_16,
			dir: P9_27
		}
	}
	/*
	Hookups that exist in hardware, but haven't been hooked up yet
	light: {
		head: P9_,
		larson_1: P9_,
		larson_2: P9_,
		larson_3: P9_
	},
	rgb_light: {
		power: {
			r: P9_,
			g: P9_,
			b: P9_
		}
	},
	sensor: {
		light_left: P9_,
		light_right: P9_
	}
	*/
};

module.exports = pins;