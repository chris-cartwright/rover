#!/usr/bin/env python

import os.path
current_dir = os.path.dirname(os.path.abspath(__file__))

import cherrypy

class Root:
    @cherrypy.expose
    def index(self):
        return open(os.path.join(current_dir, "static", "index.html")).read()

if __name__ == "__main__":
    cherrypy.quickstart(Root(), config='prod.conf')