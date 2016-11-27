import pandas as pd
import numpy as np
from sklearn import metrics
from sklearn.tree import DecisionTreeClassifier
from sklearn import tree
from sklearn import preprocessing
from sklearn.cross_validation import KFold, cross_val_score
import pydotplus


dataset = pd.read_csv('training_set_TfxIdf.csv')

# prepare datasets to be fed into the naive bayes model
# predict type given tokens
CV = dataset.COURSE.reshape((len(dataset.COURSE), 1))
data = (dataset.ix[:,range(1, CV.size + 1)].values).reshape((len(dataset.COURSE), CV.size))

feature_names = list(dataset.columns.values)
feature_names.pop(0)
class_names = list(dataset.COURSE.unique())

DT = DecisionTreeClassifier(criterion="entropy", min_samples_leaf=2)
DT = DT.fit(data, CV.ravel())
# the model
with open("predict_subject.dot", 'w') as f:
    f = tree.export_graphviz(DT, out_file=f, feature_names=feature_names, class_names=class_names, filled=True)
# predict the class for each data point
predicted = DT.predict(data)
print("Predictions: \n", np.array([predicted]).T)

# predict the probability/likelihood of the prediction
print("Probability of prediction: \n", DT.predict_proba(data))

print("Feature importance: ", DT.feature_importances_)

print("Accuracy score for the model: \n", DT.score(data, CV))

# Calculating 5 fold cross validation results
model = DecisionTreeClassifier()
kf = KFold(len(CV), n_folds=10, shuffle=True, random_state=0)
scores = cross_val_score(model, data, CV, cv=kf)
print("Accuracy of every fold in 10 fold cross validation: ", abs(scores))
print("Mean of the 10 fold cross-validation: %0.2f" % abs(scores.mean()))
print("Deviation:(+/- %0.2f)" % (scores.std() * 2))