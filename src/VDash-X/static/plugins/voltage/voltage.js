
function VoltageCtrl($scope, $element, DataModel) {
	var _settings = {
		min: 0,
		max: 0,
		precision: 2
	};
	
	$scope.init = function(data){
		$.extend(_settings, data);
		
		DataModel.$watch("sensors.battery", function(value) {
			var _dec = (value - _settings.min) / (_settings.max - _settings.min);
			var _r = Math.floor(255 - 255 * _dec);
			var _g = 255 - _r;
			var _b = 0;
			
			if(_dec < 0) {
				_r = 255;
				_g = 0;
				_b = 255;
			}
			
			if(_dec > 1) {
				_r = 0;
				_g = 255;
				_b = 255;
			}
			
			$scope.raw = value;
			$scope.percent = _dec * 100;
			$scope.color = "rgb(" + _r + ", " + _g + ", " + _b + ")";
		});
		
		$scope.precision = _settings.precision;
	};
	
	$scope.raw = 0;
	$scope.percent = 0;
}