import pandas as pd
from sklearn import metrics
import numpy as np
from sklearn.cross_validation import KFold, cross_val_score
from sklearn.naive_bayes import MultinomialNB


dataset = pd.read_csv('training_set_TfxIdf.csv')

# prepare datasets to be fed into the naive bayes model
# predict type given tokens
CV = dataset.COURSE.reshape((len(dataset.COURSE), 1))
data = (dataset.ix[:,'ponti':'sparr'].values).reshape((len(dataset.COURSE), 8971))

MB = MultinomialNB()
MB.fit(data, CV)

print("Probability of the classes: ", MB.class_log_prior_)
print("Count of the classes: ", MB.class_count_)
print("Feature Count of the classes: ", MB.feature_count_)
predicted = MB.predict(data)
print("Predictions:\n",np.array([predicted]).T)

results = open('results_TfxIdf.txt', 'w')
for entry in predicted:
    results.write(str(entry) + '\n')
results.close()
print("DONE")
