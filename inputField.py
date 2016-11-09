from tkinter import *

master = Tk()
Label(master, text="Word to Test").grid(row=0)

e1 = Entry(master)
v = e1.get()

#dictionary to keep list of tries / outputs
values = {}

# need to link to actual naive bayes output class
c = "default class"

e1.grid(row=0, column=1)

# to do saving prediction in dictionary for easy exporting of results
#wordToBeClassified = e1.get()
#classPredicted = c

#values{c} = wordToBeClassified

def clear_textbox():
    e1.delete(0, END)

# Below would output the predicted class based on input
def show_entry_fields():
    print("The word %s" % (e1.get()))
    print("is predicted to be in class %s" % (c))
    # clears input field as well on button click
    e1.delete(0, END)

Button(master, text='Quit', command=master.quit).grid(row=3, column=0, sticky=W, pady=4)
Button(master, text='Predict', command=show_entry_fields).grid(row=3, column=1, sticky=W, pady=4)

#Clear text field button
bu2 = Button(master, text='Clear', command=clear_textbox).grid(row=3, column=2, sticky=W, pady=4)

mainloop()
