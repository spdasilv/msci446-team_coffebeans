using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace _446_InvertedArray
{
    class Program
    {
        static void Main(string[] args)
        {
            //array to store all the stop wordsas
            string[] stopWords = new string[416];
            StreamReader sr1 = new StreamReader("stop_words.txt");
            string line;
            int i = 0;
            while ((line = sr1.ReadLine()) != null)
            {
                stopWords[i] = line;
                i = i + 1;
            }
            sr1.Close();

            //array to store and read in descriptions+titles 
            string[][] descrip = new String[12][];
            StreamReader sr2 = new StreamReader("testsubjdesc.txt");
            i = 0;
            while ((line = sr2.ReadLine()) != null)
            {
                descrip[i] = line.Split();
                i = i + 1;
            }
            sr2.Close();

            //dictionary of dictionary to store the word count
            Dictionary<string, Dictionary<string, int>> dictionary = new Dictionary<string, Dictionary<string, int>>();
            Dictionary<string, int> temp = new Dictionary<string, int>();
            bool exists;
            string subject = "";
            int value = 0;
            for (int j = 0; j < descrip.Length; j = j + 1)
            {
                for (int k = 0; k < descrip[j].Length; k = k + 1)
                {                
                    exists = Array.Exists(stopWords, element => element == descrip[j][k]);
                    if (exists == false)
                    {
                        if (temp.ContainsKey(descrip[j][k]))
                        {
                            value = temp[descrip[j][k]];
                            temp[descrip[j][k]] = value + 1;
                        }
                        else if (descrip[j][k] == "<subject>")
                        {
                            subject = descrip[j][k + 1];                            
                            temp.Clear();
                            break;
                        }
                        else if (descrip[j][k] == "</subject>")
                        {
                            dictionary.Add(subject, temp);
                         
                            break;
                        }
                        else
                        {
                            temp.Add(descrip[j][k], 1);
                        }
                    }
                }
            }
                      

            //output to a csv file           
            
            //using (StreamWriter writer = new StreamWriter("E:/c#/446-InvertedArray/results.csv"))
            //{
                //foreach (KeyValuePair<string, Dictionary<string, int>> kvp in dictionary)
                //{
                //    Console.Write(kvp.Key);                 
                //    foreach (KeyValuePair<string, int> kvp2 in kvp.Value)
                //    {
                //        Console.Write(kvp2.Key + " " + kvp2.Value + " ");                                               
                //    }
                //}
           // }
           
        }            
    }
}
