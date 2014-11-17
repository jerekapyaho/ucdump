import sys
import codecs
import unicodedata

input_file = codecs.open(sys.argv[1], 'rb', 'utf-8')
data = input_file.read()
input_file.close()

offset = 0
for ch in data:
    utf8_ch = ch.encode('utf-8')
    print('%08d: U+%06X %s' % (offset, ord(ch), unicodedata.name(ch, '(unnamed character)')))
    offset = offset + len(utf8_ch)
