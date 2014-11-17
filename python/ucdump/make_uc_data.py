'''
An attempt to tokenize the Unicode character database (names only)
to take less space.
'''
import csv
import sys
import codecs
import operator

DATA_FILE_NAME = 'UnicodeData.txt'
THRESHOLD = 50       # word must appear at least this many times
MIN_WORD_LENGTH = 4  # word must be at least this long to bother tokenizing
PRIVATE_BLOCK_START = 0xE000   # utilize Private Use Area characters starting with this offset
PRIVATE_BLOCK_END = 0xF8FF

def histogram(L):
    d = {}
    for x in L:
        if x in d:
            d[x] += 1
        else:
            d[x] = 1
    return d
    
def main(args):
    data_file_path = args[0]

    # All the words occurring in character names, for making a histogram
    all_words = []
    
    # All the (non-algorithmic) Unicode characters, indexed by character code. 
    all_chars = {}
    
    total_original_chars = 0  # how many characters needed for the names
    # The byte count is the same, since the names are actually in ASCII.
    
    with codecs.open(data_file_path + '/' + DATA_FILE_NAME, encoding='utf-8') as infile:
        reader = csv.reader(infile, delimiter=';')
        for columns in reader:
            # At this time we're only interested in column 2, the character name
            char_name = columns[1]
            
            if char_name.startswith('<'): # special block
                print('Skipping special block: %s' % char_name)
                continue
                
            # Well OK, we want the codepoint too. It's in the first column, in hex.
            codepoint = int(columns[0], 16)
            
            all_chars[codepoint] = char_name
            
            total_original_chars += len(char_name)
            
            name_words = char_name.split(' ')
            [all_words.append(w) for w in name_words]
            
        infile.close()
    # Now we have all the words which occur in character names.

    # Generate a histogram dictionary, with key=word, value=occurrence count
    hist = histogram(all_words)
    #print(hist)
    
    # Find out which are the most frequent or the longest words
    top_words = {}
    for w in hist:
        if len(w) < MIN_WORD_LENGTH:
            continue
        count = hist.get(w)
        if count >= THRESHOLD or len(w) >= 2*MIN_WORD_LENGTH:
            top_words[w] = count

    # Sort the words by count, most frequent first
    print(sorted(top_words.items(), key=operator.itemgetter(1), reverse=True))
        
    # Compute the space savings from storing popular words as single characters,
    # allocated from the Private Use Area.
    # Each PUA character takes three bytes in UTF-8. For example, the character
    # name "LATIN SMALL LETTER A" would be stored as "<latin><small><letter>A",
    # where <latin>, <small> and <letter> are PUA characters, with an implicit
    # trailing space after them. This would take up 3+1 characters instead of 20,
    # and encoded in UTF-8 it would require 3*3+1 = 10 bytes instead of 20 bytes.
    
    total_chars = 0
    for w in top_words:
        count = top_words.get(w)
        total_chars += count * len(w)
    print('Total characters required: %d' % total_chars)

    # Could also compute the required number of bytes in UTF-8 encoding.
    
    # Save the most frequent words in a dictionary, keyed by a sequential integer.
    # This key value will be added to the base of our Private Use Area block
    # to get the Unicode codepoint for the PUA character representing the word.
    tokens = {}
    index = 0
    max_token_count = PRIVATE_BLOCK_END - PRIVATE_BLOCK_START
    for w in top_words:
        tokens[index] = w
        index += 1
        if index > max_token_count:
            print('Number of tokens capped to %d due to the PUA size' % max_token_count)
    print('Tokenized %d characters' % len(tokens.keys()))
    
    # Make a reverse token dictionary to find index by token
    reverse_tokens = dict((v,k) for k,v in tokens.items())
    print(reverse_tokens)

    print('--- TOKEN TABLE BEGINS ---')
    for i in tokens:
        print('%X,%s' % (i, tokens[i]))
    print('--- TOKEN TABLE ENDS ---')
    
    # Just to see how this works, lets construct a list of all the Unicode characters
    # and their names, but with the names tokenized. Put the names in a dictionary,
    # keyed by codepont.
    all_chars_tokenized = {}
    
    # Compute some statistics about the space requirements
    total_chars = 0
    total_chars_tokenized = 0
    
    for cp in all_chars:
        # Get the original character name
        char_name = all_chars[cp]
        total_chars += len(char_name)
        
        # Split the original name into words
        parts = char_name.split(' ')
        
        tokenized = ''
        tokenized_parts = []
        for p in parts:
            if p in top_words:  # we need to tokenize this word
                # Find the index of this word in our token list
                token_index = reverse_tokens[p]
                
                ch = chr(PRIVATE_BLOCK_START + token_index)
                tokenized += ch
                
                tokenized_parts.append(ch)
            else:
                tokenized += p
                tokenized += ' '  # add back the space lost in the split
                tokenized_parts.append(p + ' ')
                
        all_chars_tokenized[cp] = tokenized.rstrip()
        print('%s ==> %s' % (char_name, tokenized_parts))
        
        total_chars_tokenized += len(all_chars_tokenized[cp])
                
    #print(all_chars)
    #print(all_chars_tokenized)
    
    print('Total characters (original):  %d\nTotal characters (tokenized): %d' % (total_chars, total_chars_tokenized))
    
    # Now for the acid test: reconstruct the character names using the tokens.
    # We should get exactly the same character names back.
    for cp in list(sorted(all_chars_tokenized)):
        tokenized_char_name = all_chars_tokenized[cp]
        char_name = ''
        for c in tokenized_char_name:
            codepoint = ord(c)
            if codepoint in range(PRIVATE_BLOCK_START, PRIVATE_BLOCK_START + len(tokens)):
                token = reverse_tokens[codepoint - PRIVATE_BLOCK_START]
                char_name += token
                char_name += ' '
            else:
                char_name += c
        if char_name == all_chars[cp]:
            #print('%05X: %s' % (cp, char_name.rstrip()))
            pass
        else:
            print('%05X: error in tokenization' % cp)
    
if __name__ == "__main__":
    main(sys.argv[1:])

