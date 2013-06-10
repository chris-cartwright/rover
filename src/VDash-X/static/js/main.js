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

$(function() {
	$("button").button();
	$("#logButtons").buttonset();

	$("ul.vehicles").on("click", "a", function() {
		$(this).siblings("div").slideToggle("fast");
	});

	$(".slider").css("font-size", "0.5em").each(function() {
		var s = $(this);
		s.slider(s.data());
	});
	
	$("[data-resizable='true']").each(function() {
		var $this = $(this);
		$this.resizable($this.data());
	});

	var windows = $("#windows");
	$("div.pane").pane().each(function() {
		var pane = $(this);
		windows.append(
			$("<li>")
				.append(
					$("<a>")
						.attr("href", "#")
						.text(pane.data("title"))
						.click(function() { pane.pane("open"); })
				)
		);
	});

	$("#menu").menu();
});

angular.module("VDash", []).run(function($rootScope, $http, $compile, DataModel) {
	$http({ method: "GET", url: "/static/plugins/voltage/voltage.html" }).
	success(function(data, status, headers, config) {
		$compile(data)($rootScope.$new(), function(clone, scope) {
			$("#sensors > div").append(clone);
		});
	});
	
	$rootScope.voltage = 8;
	$rootScope.$watch(function() { return DataModel.sensors.battery; }, function(value) { $rootScope.voltage = value; });
	$rootScope.$watch(function() { return $rootScope.voltage; }, function(value) { DataModel.sensors.battery = value; });
});
