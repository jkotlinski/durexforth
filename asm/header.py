# Verifies that .asm files list their Forth words in the header.
# This is intended as a convenience for human readers.

import sys

MAX_LINE_LENGTH = 80

def verify(path):
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
        start = line.find('"') + 1
        end = line.rfind('"')
        word = line[start:end]
        word = word.replace('\\"', '"')
        word = word.upper()
        words += [word]

    if not words:
        return

    expected_header = [";"]
    for word in words:
        if len(expected_header[-1]) + len(word) < MAX_LINE_LENGTH:
            expected_header[-1] += ' ' + word
        else:
            expected_header[-1] += '\n'
            expected_header += ["; " + word]
    expected_header[-1] += '\n'

    if found_header == expected_header:
        return

    sys.exit(path + " -- found outdated header! Change it to:\n" + ''.join(expected_header))

for path in sys.argv[1:]:
    verify(path)
