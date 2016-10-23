using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffe_Beans_Inverted_Index
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] stopWords = Stop_Words("Stop Words.txt");

            StreamReader read_subjects = new StreamReader("Subject_DescV2.txt");
            string line = read_subjects.ReadLine();
            string curr_subject = "";
            string[] curr_line = null;
            Dictionary<string, Dictionary<string, int>> subject_Collection = new Dictionary<string, Dictionary<string, int>>();

            while (line != null)
            {
                curr_line = line.Split();

                if (curr_line[0] == "<SUBJECT>")
                {
                    curr_subject = curr_line[1];
                    if (curr_subject == ",")
                    {
                        Console.WriteLine("FOUND IT!");
                    }
                    subject_Collection.Add(curr_subject, new Dictionary<string, int>());
                }
                else if (curr_line[0] == "</SUBJECT>")
                {
                    line = read_subjects.ReadLine();
                    continue;
                }
                else
                {
                    for (int i = 0; i < curr_line.Length; i = i + 1)
                    {
                        if (Array.Exists(stopWords, element => element == curr_line[i].ToLower()) == false)
                        {
                            if (subject_Collection[curr_subject].ContainsKey(curr_line[i].ToLower()))
                            {
                                ++subject_Collection[curr_subject][curr_line[i].ToLower()];
                            }
                            else
                            {
                                subject_Collection[curr_subject].Add(curr_line[i].ToLower(), 1);
                            }
                        }
                    }
                }
                line = read_subjects.ReadLine();
            }
            read_subjects.Close();

            Dictionary<string, Dictionary<string, int>> inverted_Index = new Dictionary<string, Dictionary<string, int>>();
            foreach(KeyValuePair<string, Dictionary <string, int>> entry in subject_Collection)
            {
                string subject_Name = entry.Key;
                foreach(KeyValuePair<string, int> term in entry.Value)
                {
                    if (inverted_Index.ContainsKey(term.Key))
                    {
                        inverted_Index[term.Key].Add(subject_Name, term.Value);
                    }
                    else
                    {

                        inverted_Index.Add(term.Key, new Dictionary<string, int>
                        {
                            {subject_Name, term.Value }
                        });
                    }
                }
            }

            StreamWriter w = new StreamWriter("inverted_index.txt");
            foreach (KeyValuePair< string, Dictionary < string, int>> term in inverted_Index)
            {
                w.Write(term.Key + " : ");
                foreach(KeyValuePair<string,int> subject in term.Value)
                {
                    w.Write(subject.Key + "|" + subject.Value + "; ");
                }
                w.WriteLine();
            }

            Console.WriteLine("DONE");

        }

        static string[] Stop_Words (string file_name)
        {
            //array to store all the stop word
            string[] stopWords = new string[416];
            StreamReader sr1 = new StreamReader(file_name);
            string line;
            int i = 0;
            while ((line = sr1.ReadLine()) != null)
            {
                stopWords[i] = line;
                i = i + 1;
            }
            sr1.Close();

            return stopWords;
        }
    }
}
