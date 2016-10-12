from uwaterlooapi import UWaterlooAPI


class Course(object):
    Title = ""
    Description = ""

    def __init__(self, title, description):
        self.Title = title
        self.Description = description

file = open('Subjects.txt', 'r')
subjects_codes = file.read().split(',')

subjects = {}

uw = UWaterlooAPI(api_key="38ff9b7728efc98ac1dde8624db6755e")
for code in subjects_codes:
    subject = uw.courses(code)
    courses = {}
    for course in subject:
        course_info = Course(course['title'], course['description'])
        courses[course['course_id']] = course_info
    if len(courses) > 0:
        subjects[code] = courses
print('COMPLETED Step 1 of 2')

subject_desc = open('Subject_Desc.txt', 'w')
for k1, v1 in subjects.items():
    subject_desc.write('<SUBJECT> %s\n' % k1)
    subject_desc.write('\n')
    for k2, v2 in v1.items():
        subject_desc.write('<TITLE>\n')
        subject_desc.write('%s\n' % v2.Title)
        subject_desc.write('</TITLE>\n')
        subject_desc.write('<DESCRIPTION>\n')
        subject_desc.write('%s\n' % v2.Description)
        subject_desc.write('</DESCRIPTION>\n')
        subject_desc.write('\n')
    subject_desc.write('</SUBJECT>\n')

subject_desc.close()
print('COMPLETED Step 2 of 2')
