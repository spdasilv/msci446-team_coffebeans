import pandas as pd
from sklearn import metrics
from sklearn.cluster import KMeans
import numpy as np

dataset = pd.read_csv('training_set_TfxIdf.csv')

# prepare datasets to be fed into the naive bayes model
# predict type given tokens
CV = dataset.COURSE.reshape((len(dataset.COURSE), 1))
data = (dataset.ix[:,range(1, CV.size + 1)].values).reshape((len(dataset.COURSE), CV.size))

kmeans = KMeans(n_clusters=5, init='k-means++', random_state=170)
kmeans = kmeans.fit(data)
clusters = kmeans.labels_

print("Output Clusters")
clustering = open('Clusters.txt', 'w')
for entry in clusters:
    clustering.write(str(entry) + '\n')
clustering.close()
