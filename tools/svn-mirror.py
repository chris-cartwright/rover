#!/usr/bin/env python

import urllib, urllib2, re, argparse, os

parser = argparse.ArgumentParser(description="Mirror a Subversion folder.")
parser.add_argument("url", type=str, help="URL of folder to mirror")

args = parser.parse_args()

link = re.compile("<a href=\"(.*?)\">", re.I)

def dl(url, out, depth):
	response = urllib2.urlopen(url)
	x = 0
	for match in link.findall(response.read()):
		if match == "../":
			continue

		x += 1
		if depth > 0:
			print "\t" * depth,

		print "%2d " % (x), match
		
		if match[-1] == '/':
			os.mkdir(out + match)
			dl(url + match, out + match, depth + 1)
		else:
			urllib.urlretrieve(url + match, out + match)

dl(args.url, "./", 0)
