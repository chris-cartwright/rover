
angular.module("VDash").factory("DataModel", function($rootScope) {
	var scope = $rootScope.$new();
	scope.sensors = {
		battery: 6
	};
	
	return scope;
});