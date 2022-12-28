import math

for d in xrange(360):
    if d % 4 == 0:
        print
    a = int(32767.5 + 32767.5 * math.sin(d * math.pi / 180))
    print hex(a)[2:], ",",
