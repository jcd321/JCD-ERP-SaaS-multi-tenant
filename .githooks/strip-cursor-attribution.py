import sys

for line in sys.stdin:
    if "cursoragent@cursor.com" in line:
        continue
    if line.startswith("Made-with: Cursor"):
        continue
    sys.stdout.write(line)
