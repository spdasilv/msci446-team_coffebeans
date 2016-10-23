using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coffe_Beans_Inverted_Index
{
    public class PorterStemmer
    {

        // The passed in word turned into a char array. 
        // Quicker to use to rebuilding strings each time a change is made.
        private char[] wordArray;

        // Current index to the end of the word in the character array. This will
        // change as the end of the string gets modified.
        private int endIndex;

        // Index of the (potential) end of the stem word in the char array.
        private int stemIndex;


        /// <summary>
        /// Stem the passed in word.
        /// </summary>
        /// <param name="word">Word to evaluate</param>
        /// <returns></returns>
        public string StemWord(string word)
        {

            // Do nothing for empty strings or short words.
            if (string.IsNullOrWhiteSpace(word) || word.Length <= 2) return word;

            wordArray = word.ToCharArray();

            stemIndex = 0;
            endIndex = word.Length - 1;
            Step1();
            Step2();
            Step3();
            Step4();
            Step5();
            Step6();

            var length = endIndex + 1;
            return new String(wordArray, 0, length);
        }


        // Step1() gets rid of plurals and -ed or -ing.
        /* Examples:
			   caresses  ->  caress
			   ponies    ->  poni
			   ties      ->  ti
			   caress    ->  caress
			   cats      ->  cat

			   feed      ->  feed
			   agreed    ->  agree
			   disabled  ->  disable

			   matting   ->  mat
			   mating    ->  mate
			   meeting   ->  meet
			   milling   ->  mill
			   messing   ->  mess

			   meetings  ->  meet  		*/
        private void Step1()
        {
            // If the word ends with s take that off
            if (wordArray[endIndex] == 's')
            {
                if (EndsWith("sses"))
                {
                    endIndex -= 2;
                }
                else if (EndsWith("ies"))
                {
                    SetEnd("i");
                }
                else if (wordArray[endIndex - 1] != 's')
                {
                    endIndex--;
                }
            }
            if (EndsWith("eed"))
            {
                if (MeasureConsontantSequence() > 0)
                    endIndex--;
            }
            else if ((EndsWith("ed") || EndsWith("ing")) && VowelInStem())
            {
                endIndex = stemIndex;
                if (EndsWith("at"))
                    SetEnd("ate");
                else if (EndsWith("bl"))
                    SetEnd("ble");
                else if (EndsWith("iz"))
                    SetEnd("ize");
                else if (IsDoubleConsontant(endIndex))
                {
                    endIndex--;
                    int ch = wordArray[endIndex];
                    if (ch == 'l' || ch == 's' || ch == 'z')
                        endIndex++;
                }
                else if (MeasureConsontantSequence() == 1 && IsCVC(endIndex)) SetEnd("e");
            }
        }

        // Step2() turns terminal y to i when there is another vowel in the stem.
        private void Step2()
        {
            if (EndsWith("y") && VowelInStem())
                wordArray[endIndex] = 'i';
        }

        // Step3() maps double suffices to single ones. so -ization ( = -ize plus
        // -ation) maps to -ize etc. note that the string before the suffix must give m() > 0. 
        private void Step3()
        {
            if (endIndex == 0) return;

            /* For Bug 1 */
            switch (wordArray[endIndex - 1])
            {
                case 'a':
                    if (EndsWith("ational")) { ReplaceEnd("ate"); break; }
                    if (EndsWith("tional")) { ReplaceEnd("tion"); }
                    break;
                case 'c':
                    if (EndsWith("enci")) { ReplaceEnd("ence"); break; }
                    if (EndsWith("anci")) { ReplaceEnd("ance"); }
                    break;
                case 'e':
                    if (EndsWith("izer")) { ReplaceEnd("ize"); }
                    break;
                case 'l':
                    if (EndsWith("bli")) { ReplaceEnd("ble"); break; }
                    if (EndsWith("alli")) { ReplaceEnd("al"); break; }
                    if (EndsWith("entli")) { ReplaceEnd("ent"); break; }
                    if (EndsWith("eli")) { ReplaceEnd("e"); break; }
                    if (EndsWith("ousli")) { ReplaceEnd("ous"); }
                    break;
                case 'o':
                    if (EndsWith("ization")) { ReplaceEnd("ize"); break; }
                    if (EndsWith("ation")) { ReplaceEnd("ate"); break; }
                    if (EndsWith("ator")) { ReplaceEnd("ate"); }
                    break;
                case 's':
                    if (EndsWith("alism")) { ReplaceEnd("al"); break; }
                    if (EndsWith("iveness")) { ReplaceEnd("ive"); break; }
                    if (EndsWith("fulness")) { ReplaceEnd("ful"); break; }
                    if (EndsWith("ousness")) { ReplaceEnd("ous"); }
                    break;
                case 't':
                    if (EndsWith("aliti")) { ReplaceEnd("al"); break; }
                    if (EndsWith("iviti")) { ReplaceEnd("ive"); break; }
                    if (EndsWith("biliti")) { ReplaceEnd("ble"); }
                    break;
                case 'g':
                    if (EndsWith("logi"))
                    {
                        ReplaceEnd("log");
                    }
                    break;
            }
        }

        /* step4() deals with -ic-, -full, -ness etc. similar strategy to step3. */
        private void Step4()
        {
            switch (wordArray[endIndex])
            {
                case 'e':
                    if (EndsWith("icate")) { ReplaceEnd("ic"); break; }
                    if (EndsWith("ative")) { ReplaceEnd(""); break; }
                    if (EndsWith("alize")) { ReplaceEnd("al"); }
                    break;
                case 'i':
                    if (EndsWith("iciti")) { ReplaceEnd("ic"); }
                    break;
                case 'l':
                    if (EndsWith("ical")) { ReplaceEnd("ic"); break; }
                    if (EndsWith("ful")) { ReplaceEnd(""); }
                    break;
                case 's':
                    if (EndsWith("ness")) { ReplaceEnd(""); }
                    break;
            }
        }

        /* step5() takes off -ant, -ence etc., in context <c>vcvc<v>. */
        private void Step5()
        {
            if (endIndex == 0) return;

            switch (wordArray[endIndex - 1])
            {
                case 'a':
                    if (EndsWith("al")) break; return;
                case 'c':
                    if (EndsWith("ance")) break;
                    if (EndsWith("ence")) break; return;
                case 'e':
                    if (EndsWith("er")) break; return;
                case 'i':
                    if (EndsWith("ic")) break; return;
                case 'l':
                    if (EndsWith("able")) break;
                    if (EndsWith("ible")) break; return;
                case 'n':
                    if (EndsWith("ant")) break;
                    if (EndsWith("ement")) break;
                    if (EndsWith("ment")) break;
                    /* element etc. not stripped before the m */
                    if (EndsWith("ent")) break; return;
                case 'o':
                    if (EndsWith("ion") && stemIndex >= 0 && (wordArray[stemIndex] == 's' || wordArray[stemIndex] == 't')) break;
                    /* j >= 0 fixes Bug 2 */
                    if (EndsWith("ou")) break; return;
                /* takes care of -ous */
                case 's':
                    if (EndsWith("ism")) break; return;
                case 't':
                    if (EndsWith("ate")) break;
                    if (EndsWith("iti")) break; return;
                case 'u':
                    if (EndsWith("ous")) break; return;
                case 'v':
                    if (EndsWith("ive")) break; return;
                case 'z':
                    if (EndsWith("ize")) break; return;
                default:
                    return;
            }
            if (MeasureConsontantSequence() > 1)
                endIndex = stemIndex;
        }

        /* step6() removes a final -e if m() > 1. */
        private void Step6()
        {
            stemIndex = endIndex;

            if (wordArray[endIndex] == 'e')
            {
                var a = MeasureConsontantSequence();
                if (a > 1 || a == 1 && !IsCVC(endIndex - 1))
                    endIndex--;
            }
            if (wordArray[endIndex] == 'l' && IsDoubleConsontant(endIndex) && MeasureConsontantSequence() > 1)
                endIndex--;
        }

        // Returns true if the character at the specified index is a consonant.
        // With special handling for 'y'.
        private bool IsConsonant(int index)
        {
            var c = wordArray[index];
            if (c == 'a' || c == 'e' || c == 'i' || c == 'o' || c == 'u') return false;
            return c != 'y' || (index == 0 || !IsConsonant(index - 1));
        }

        /* m() measures the number of consonant sequences between 0 and j. if c is
		   a consonant sequence and v a vowel sequence, and <..> indicates arbitrary
		   presence,

			  <c><v>       gives 0
			  <c>vc<v>     gives 1
			  <c>vcvc<v>   gives 2
			  <c>vcvcvc<v> gives 3
			  ....		*/
        private int MeasureConsontantSequence()
        {
            var n = 0;
            var index = 0;
            while (true)
            {
                if (index > stemIndex) return n;
                if (!IsConsonant(index)) break; index++;
            }
            index++;
            while (true)
            {
                while (true)
                {
                    if (index > stemIndex) return n;
                    if (IsConsonant(index)) break;
                    index++;
                }
                index++;
                n++;
                while (true)
                {
                    if (index > stemIndex) return n;
                    if (!IsConsonant(index)) break;
                    index++;
                }
                index++;
            }
        }

        // Return true if there is a vowel in the current stem (0 ... stemIndex)
        private bool VowelInStem()
        {
            int i;
            for (i = 0; i <= stemIndex; i++)
            {
                if (!IsConsonant(i)) return true;
            }
            return false;
        }

        // Returns true if the char at the specified index and the one preceeding it are the same consonants.
        private bool IsDoubleConsontant(int index)
        {
            if (index < 1) return false;
            return wordArray[index] == wordArray[index - 1] && IsConsonant(index);
        }

        /* cvc(i) is true <=> i-2,i-1,i has the form consonant - vowel - consonant
		   and also if the second c is not w,x or y. this is used when trying to
		   restore an e at the end of a short word. e.g.

			  cav(e), lov(e), hop(e), crim(e), but
			  snow, box, tray.		*/
        private bool IsCVC(int index)
        {
            if (index < 2 || !IsConsonant(index) || IsConsonant(index - 1) || !IsConsonant(index - 2)) return false;
            var c = wordArray[index];
            return c != 'w' && c != 'x' && c != 'y';
        }

        // Does the current word array end with the specified string.
        private bool EndsWith(string s)
        {
            var length = s.Length;
            var index = endIndex - length + 1;
            if (index < 0) return false;

            for (var i = 0; i < length; i++)
            {
                if (wordArray[index + i] != s[i]) return false;
            }
            stemIndex = endIndex - length;
            return true;
        }

        // Set the end of the word to s.
        // Starting at the current stem pointer and readjusting the end pointer.
        private void SetEnd(string s)
        {
            var length = s.Length;
            var index = stemIndex + 1;
            for (var i = 0; i < length; i++)
            {
                wordArray[index + i] = s[i];
            }
            // Set the end pointer to the new end of the word.
            endIndex = stemIndex + length;
        }

        // Conditionally replace the end of the word
        private void ReplaceEnd(string s)
        {
            if (MeasureConsontantSequence() > 0) SetEnd(s);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            List<string> stopWords = Stop_Words("Stop Words.txt");

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
                    subject_Collection[curr_subject] = Tokenize(line, subject_Collection[curr_subject], stopWords);
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

        // This method takes a string and strips all
        // special character of the string. If a number
        // is found close to a letter, both characters
        // are then split.
        public static string RemoveSpecialCharacters(string str)
        {
            StringBuilder sb = new StringBuilder();
            char prev = ' ';
            foreach (char c in str)
            {
                if ((c >= '0' && c <= '9') ||
                    (c >= 'a' && c <= 'z'))
                {
                    if (((c >= 'a' && c <= 'z') &&
                        (prev >= '0' && prev <= '9')) ||
                        ((prev >= 'a' && prev <= 'z') &&
                        (c >= '0' && c <= '9')))
                    {
                        sb.Append(' ');
                    }
                    sb.Append(c);
                    prev = c;
                }
                else if (prev != ' ')
                {
                    sb.Append(' ');
                    prev = ' ';
                }
            }
            return sb.ToString();
        }

        // This is the Custom Tokenization model.
        // Initially the text to be tokenized is transformed into a sequence
        // of lowercase characters and split on whitespace.Each word is then
        // stripped of any special character while numbers and letters are
        // separated.As an example “international-no02” is split into the
        // tokens “international”, “no”, “02”. For each token we verify if
        // they are contained within a list of 418 stop words, and, if they
        // are the token is thrown away and will not be added to the inverted
        // index nor the compounded token.Each token is then stemmed and then
        // united once more to generate the compounded token.At the end of the
        // process “International-No02” is divided into 4 tokens: “intern”, 
        // “no”, “02” and “internno02”.
        static public Dictionary<string, int>
           Tokenize(string text, Dictionary<string, int> tmp_Token,
            List<string> stop_words)
        {
            List<string> tokens = new List<string>();
            text = text.ToLower();
            var stemmer = new PorterStemmer();
            string stem = "";
            string[] text_arr = text.Split(' ');
            string word = "";
            string[] word_arr;
            for (int j = 0; j < text_arr.Length; ++j)
            {
                int compund_count = 0;
                string compound = "";
                word = RemoveSpecialCharacters(text_arr[j]);
                word = word.Trim();
                word_arr = word.Split(new char[0]);
                for (int g = 0; g < word_arr.Length; ++g)
                {
                    if (!stop_words.Contains(word_arr[g])
                        && word_arr[g] != "")
                    {
                        stem = stemmer.StemWord(word_arr[g]);
                        tokens.Add(stem);
                        compound += stem;
                        ++compund_count;
                    }
                }
                if (compund_count > 1)
                {
                    tokens.Add(compound);
                }

            }
            for (int j = 0; j < tokens.Count; ++j)
            {
                if (!tmp_Token.ContainsKey(tokens[j]))
                {
                    tmp_Token.Add(tokens[j], 1);
                }
                else
                {
                    tmp_Token[tokens[j]] += 1;
                }
            }

            return tmp_Token;
        }

        static List<string> Stop_Words (string file_name)
        {
            //array to store all the stop word
            List<string> stopWords = new List<string>();
            StreamReader sr1 = new StreamReader(file_name);
            string line;
            while ((line = sr1.ReadLine()) != null)
            {
                stopWords.Add(line);
            }
            sr1.Close();

            return stopWords;
        }
    }
}
