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
    tokens = tokenizer.tokenize(words.lower())
    for token in tokens:
        if token in stop_words:
            continue
        token = stemmer.stem(token)
        if token in word_dict:
            word_dict[token] += 1
        else:
            word_dict[token] = 1
    return word_dict


curr_subject = ""
curr_line = ""
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
        continue
    else:
        word_collection = tokenize(word_collection, line, stop_words)
        course_description.Courses = tokenize(course_description.Courses, line, stop_words)
read_subjects.close()

word_list = list(word_collection.items())
train_set = open('training_set.txt', 'w')
train_set.write('COURSE,')
for i, item in enumerate(word_list):
    if (i+1) == len(word_list):
        train_set.write(item[0] + '\n')
    else:
        train_set.write(item[0] + ',')

for entry in subject_Collection:
    train_set.write(entry.Subject + ',')
    for i, item in enumerate(word_list):
        if (i+1) == len(word_list):
            if item[0] in entry.Courses.keys():
                train_set.write(str(entry.Courses[item[0]]) + '\n')
            else:
                train_set.write('0\n')
        else:
            if item[0] in entry.Courses.keys():
                train_set.write(str(entry.Courses[item[0]]) + ',')
            else:
                train_set.write('0,')
train_set.close()
print("DONE")
