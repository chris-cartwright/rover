
var Logger = {};
(function($, angular) {
	function _log(level, msg) {
		var _elem = angular.element($("[ng-app]"));
		var _logger = _elem.injector().get("Logger");
		_logger.log(level, msg);
		_elem.scope().$apply();
	}
	
	Logger.Error = function(msg) {
		_log("Error", msg);
	};
	
	Logger.Warn = function(msg) {
		_log("Warn", msg);
	};
	
	Logger.Info = function(msg) {
		_log("Info", msg);
	};
	
	Logger.Debug = function(msg) {
		_log("Debug", msg);
	};
})(jQuery, angular);

function LoggerCtrl($scope, $element, Logger) {
	$scope.logs = [];
	$scope.level = "";
	
	$scope.levels = ["Error", "Warn", "Info", "Debug"];
	
	$scope.timeInt = function(log) {
		return log.time.getTime();
	};
	
	$scope.form = {
		level: "Error",
		msg: ""
	};
	
	$scope.addLog = function() {
		Logger.log($scope.form.level, $scope.form.msg);
		
		$scope.form.level = "Error";
		$scope.form.msg = "";
	};
	
	$scope.$watch(function() { return Logger.logs; }, function(value) { $scope.logs = value; });
	$scope.$watch(function() { return Logger.level; }, function(value) { $scope.level = value; });
	$scope.$watch(function() { return $scope.level; }, function(value) { Logger.level = value; });
}