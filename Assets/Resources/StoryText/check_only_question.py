import os

files = os.listdir("./")

for f in files:
    print f
    with open(f, 'r') as story:

        for line in story:
            if line.startswith("<q"):
                print line
                exit(0)
