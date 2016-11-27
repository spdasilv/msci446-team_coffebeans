import math
from nltk.stem.porter import *
from nltk.tokenize import RegexpTokenizer


class WordCount(object):
    Subject = ""
    Courses = {}

    def __init__(self, subject, courses):
        self.Subject = subject
        self.Courses = courses


def get_stop_words():
    stop_list = []
    file = open('Stop Words.txt', 'r')
    for stop in file:
        stop_list.append(stop.replace("\n", ""))
    file.close()
    return stop_list


def tokenize(word_dict, words, stop_words):
    stemmer = PorterStemmer()
    tokenizer = RegexpTokenizer(r'\w+')
    words = ''.join([i for i in words if not i.isdigit()])
    tokens = tokenizer.tokenize(words.lower())
    for token in tokens:
        if token in stop_words:
            continue
        token = stemmer.stem(token)
        token = strip_non_ascii(token)
        if token in word_dict:
            word_dict[token] += 1
        else:
            word_dict[token] = 1
    return word_dict


def increase_frequency(word_dict, doc_freq):
    for word in word_dict:
        if word in doc_freq:
            doc_freq[word] += 1
        else:
            doc_freq[word] = 1
    return doc_freq


def strip_non_ascii(string):
    # Returns the string without non ASCII characters
    stripped = (c for c in string if 0 < ord(c) < 127)
    return ''.join(stripped)

curr_subject = ""
curr_line = ""
document_frequency = {}
word_collection = {}
subject_Collection = []
stop_words = get_stop_words()

read_subjects = open('Subject_Desc.txt', 'r')
for line in read_subjects:
    line = line.replace("\n", "")
    curr_line = line.split(' ')
    if curr_line[0] == "<SUBJECT>":
        curr_subject = curr_line[1]
    elif curr_line[0] == "</SUBJECT>":
        continue
    elif curr_line[0] == "<COURSE>":
        course_description = WordCount(curr_subject, {})
    elif curr_line[0] == "</COURSE>":
        subject_Collection.append(course_description)
        document_frequency = increase_frequency(course_description.Courses, document_frequency)
        continue
    else:
        word_collection = tokenize(word_collection, line, stop_words)
        course_description.Courses = tokenize(course_description.Courses, line, stop_words)
read_subjects.close()

word_list = list(word_collection.items())
train_set = open('association_set_TfxIdf.txt', 'w')
train_set.write('COURSE,Top_1,Top_2,Top_3,Top_4,Top_5,Top_6,Top_7,Top_8,Top_9,Top_10\n')

for entry in subject_Collection:
    train_set.write(entry.Subject + ',')
    sorted_row = []
    for key, value in entry.Courses.items():
        tf_idf = value*math.log(len(subject_Collection)/document_frequency[key])
        sorted_row.append((key, tf_idf))
    sorted_row.sort(key=lambda tup: tup[1])
    sorted_row.reverse()
    for i in range(0, 10):
        if i < 9:
            if i < len(sorted_row):
                term = sorted_row[i][0]
                train_set.write(term + ',')
            else:
                train_set.write(',')
        else:
            if i < len(sorted_row):
                term = sorted_row[i][0]
                train_set.write(term + '\n')
            else:
                train_set.write('\n')
train_set.close()
print("DONE")
