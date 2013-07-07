
angular.module("VDash")
.factory("DataModel", function($rootScope) {
	var _dataModel = $rootScope.$new();
	_dataModel.sensors = {
		battery: 6
	};
	
	return _dataModel;
})
.factory("Logger", function($rootScope) {
	var _logger = $rootScope.$new();
	_logger.logs = [{
		level: "Debug",
		message: "Logger service initialized.",
		time: new Date()
	}];
	_logger.level = "";
	_logger.log = function(level, msg) {
		_logger.logs.push({ level: level, message: msg, time: new Date() });
	};
	
	return _logger;
});