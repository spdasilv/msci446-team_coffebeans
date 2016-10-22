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
            string[] stopWords = Stop_Words("stop_words.txt");

            StreamReader read_subjects = new StreamReader("testsubjdesc.txt");
            string line = read_subjects.ReadLine();
            string curr_subject = "";
            while (line != null)
            {
                //CODE HERE

                line = read_subjects.ReadLine();
            }
        }

        static string[] Stop_Words (string file_name)
        {
            //array to store all the stop wordsas
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
