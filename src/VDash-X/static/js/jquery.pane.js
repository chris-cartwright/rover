/*
Copyright (C) 2012 Christopher Cartwright
    
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
(function($) {
	var defaults = {
		speed: "fast",
		title: "Unknown",
		collapsible: true
	};

	function Pane(el, opts) {
		var self = this;

		self.el = $(el);
		self.settings = $.extend({}, defaults, opts, self.el.data());
		self.visible = true;
		self.collapsed = false;
		self.content = $("<div>");

		self.el.prepend(self.content);
		self.content.append(self.el.children().slice(1));

		var h2 = $("<h2>").text(self.settings.title);
		h2.append(
			$("<a>")
				.addClass("ui-icon-close")
				.click(function() { self.close(); })
		);

		if(self.settings.collapsible) {
			self.minmax = $("<a>")
				.addClass("ui-icon-minus")
				.click(function() { self.toggle(); });
			h2.append(self.minmax);
		}

		h2.find("a")
			.attr("href", "#")
			.addClass("ui-icon")
			.hover(
				function() { $(this).addClass("ui-state-hover"); },
				function() { $(this).removeClass("ui-state-hover"); }
			);
				

		self.el.prepend(h2);

		this.show = function() {
			self.collapsed = false;
			self.minmax.removeClass("ui-icon-plus").addClass("ui-icon-minus");
			self.content.slideDown(self.settings.speed);
		};

		this.hide = function() {
			self.collapsed = true;
			self.minmax.removeClass("ui-icon-minus").addClass("ui-icon-plus");
			self.content.slideUp(self.settings.speed);
		};

		this.toggle = function() {
			if(!self.collapsed) {
				self.hide();
			}
			else {
				self.show();
			}
		};

		this.flash = function() {
			self.el.addClass("focus");
			setTimeout(function() {
				self.el.removeClass("focus", 500);
			}, 250);
		};

		this.open = function() {
			if(self.visible) {
				self.flash();
				return;
			}

			self.visible = true;
			self.el.slideDown(self.settings.speed);
		};

		this.close = function() {
			self.visible = false;
			self.el.slideUp(self.settings.speed);
		};
	}

	$.fn.pane = function(method) {
		return this.each(function() {
			var el = $.data(this, "plugin_Pane");
			if(el && $.isFunction(el[method])) {
				el[method].apply(el, Array.prototype.slice.call(arguments, 1));
			}
			else if(typeof(method) === "object" || method == undefined) {
				$.data(this, "plugin_Pane", new Pane(this, method));
			}
			else {
				$.error("Method " + method + " does not exist.");
			}
		});
	};
})(jQuery);
