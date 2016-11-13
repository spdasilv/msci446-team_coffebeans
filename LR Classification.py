import pandas as pd
from sklearn import metrics
import numpy as np
from sklearn.cross_validation import KFold, cross_val_score
from sklearn.linear_model import LogisticRegression




dataset = pd.read_csv('training_set_TfxIdf.csv')

# prepare datasets to be fed into the naive bayes model
# predict type given tokens
CV = dataset.COURSE.reshape((len(dataset.COURSE), 1))
data = (dataset.ix[:,range(1, CV.size + 1)].values).reshape((len(dataset.COURSE), CV.size))

LR = LogisticRegression()
LR.fit(data, CV.ravel())

predicted = LR.predict(data)
print("Predictions:\n", np.array([predicted]).T)

# predict the probability/likelihood of the prediction
prob_of_pred = LR.predict_proba(data)
print("Probability of each class for the prediction: \n", prob_of_pred)
print("Accuracy of the model: ", LR.score(data, CV))

# Calculating 10 fold cross validation results
model = LogisticRegression()
kf = KFold(len(CV), n_folds=10, shuffle=True, random_state=0)
scores = cross_val_score(model, data, CV.ravel(), cv=kf)
print("MSE of every fold in 10 fold cross validation: ", abs(scores))
print("Mean of the 10 fold cross-validation: %0.2f" % abs(scores.mean()))

results = open('results_TfxIdf_LR.txt', 'w')
for entry in predicted:
    results.write(str(entry) + '\n')
results.close()

print("DONE")
