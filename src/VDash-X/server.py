#!/usr/bin/env python

# Copyright (C) 2013 Christopher Cartwright
#
# This file is part of VDash.
#
# VDash is free software: you can redistribute it and/or modify
# it under the terms of the GNU General Public License as published by
# the Free Software Foundation, either version 3 of the License, or
# (at your option) any later version.
#
# VDash is distributed in the hope that it will be useful,
# but WITHOUT ANY WARRANTY; without even the implied warranty of
# MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
# GNU General Public License for more details.
#
# You should have received a copy of the GNU General Public License
# along with VDash.  If not, see <http://www.gnu.org/licenses/>.

import os.path
current_dir = os.path.dirname(os.path.abspath(__file__))

import cherrypy

class Root:
    @cherrypy.expose
    def index(self):
        return open(os.path.join(current_dir, "static", "index.html")).read()

if __name__ == "__main__":
    cherrypy.quickstart(Root(), config='prod.conf')