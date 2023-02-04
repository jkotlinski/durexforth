# Verifies that .asm files list its Forth words in the header.

import sys

MAX_LINE_LENGTH = 80

path = sys.argv[1]
lines = open(path).readlines()

found_header = []
for line in lines:
    if line[0] == ";":
        found_header += [line]
    else:
        break

words = []
for line in lines:
    if "+BACKLINK" not in line:
        continue
    words += [line.split('"')[1].rsplit('"')[0].upper()]

if not words:
    sys.exit(0)

expected_header = [";"]
for word in words:
    if len(expected_header[-1]) + len(word) < MAX_LINE_LENGTH:
        expected_header[-1] += ' ' + word
    else:
        expected_header[-1] += '\n'
        expected_header += ["; " + word]
expected_header[-1] += '\n'

if found_header == expected_header:
    sys.exit(0)

msg = path + " -- outdated header! expected:\n\n"
msg += ''.join(expected_header)

sys.exit(msg)
