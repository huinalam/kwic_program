using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace kwic_program
{
    class Program
    {
        static void Main(string[] args)
        {
            string text = File.ReadAllText(@"TheLastQuestion.txt");

            var kwic = new KWIC(text)
            {
                ForwardMargin = 30,
                BackwardMargin = 30
            };
            var results = kwic.Concordance("question");

            foreach (var result in results)
            {
                Console.WriteLine(result);
            }
        }

        public class KWIC
        {
            private readonly char[] _delimiterChars;
            public string OrginString { get; private set; }
            
            public int ForwardMargin
            {
                get { return _forwardMargin; }
                set
                {
                    if (value < 0)
                        throw new ArgumentOutOfRangeException("need to natural number");
                    _forwardMargin = value;
                }
            }
            private int _forwardMargin = 0;

            public int BackwardMargin
            {
                get { return _backwardMargin; }
                set
                {
                    if (value < 0)
                        throw new ArgumentOutOfRangeException("need to natural number");
                    _backwardMargin = value;
                }
            }
            private int _backwardMargin = 0;

            private List<string> _wordList { get; set; }
            private List<WordSet> _wordSetList { get; set; } = new List<WordSet>();



            public KWIC(string text) : this(text, DelimiterChars.Basic)
            {
            }

            public KWIC(string text, char[] delimiterChars)
            {
                if (text == null)
                {
                    throw new ArgumentNullException(nameof(text));
                }

                BackwardMargin = ForwardMargin = 30;
                OrginString = text;
                SplitWords(text);
                _delimiterChars = delimiterChars;
            }

            private void SplitWords(string text)
            {
                _wordList = text.Split(_delimiterChars).ToList();

                int idx = 0;
                foreach (var word in _wordList)
                {
                    _wordSetList.Add(new WordSet
                    {
                        Index = idx++,
                        Word = word
                    });
                }
            }

            /// <summary>
            /// Searching Key Word In Context.
            /// </summary>
            /// <param name="target">word for searching</param>
            /// <returns>return format for concordance lines. if dont find word, return null.</returns>
            public List<string> Concordance(string target)
            {
                var searchedWords = 
                    _wordSetList.Where(q => q.Word == target).ToList();

                return searchedWords.Count == 0 ?
                    null : 
                    searchedWords.Select(PrintLine).ToList();
            }

            private string PrintLine(WordSet wordSet)
            {
                var forwardwords = BuildForwardWords(wordSet, ForwardMargin);
                var backwardWords = BuildBackwardWords(wordSet, BackwardMargin);
                
                var line = new StringBuilder();
                line.Append(forwardwords);
                line.Append(wordSet.Word);
                line.Append(backwardWords);
                return line.ToString();
            }

            private string BuildForwardWords(WordSet wordSet, int forwardMargin)
            {
                int charCount = 0;
                var forwardList = new List<string>();
                for (var idx = wordSet.Index - 1; idx > 0; idx--)
                {
                    if (GetWords(idx, forwardMargin, forwardList, ref charCount)) break;
                }
                forwardList.Reverse();

                var forwardStrBuilder = new StringBuilder();
                for (var idx = 0; idx < forwardMargin - charCount; idx++)
                {
                    forwardStrBuilder.Append(" ");
                }
                foreach (var word in forwardList)
                {
                    forwardStrBuilder.Append(word);
                    forwardStrBuilder.Append(" ");
                }
                return forwardStrBuilder.ToString();
            }

            private string BuildBackwardWords(WordSet wordSet, int backwardMargin)
            {
                int charCount = 0;
                var backwardList = new List<string>();
                for (var idx = wordSet.Index + 1; idx < _wordList.Count; idx++)
                {
                    if (GetWords(idx, backwardMargin, backwardList, ref charCount)) break;
                }

                var forwardStrBuilder = new StringBuilder();
                forwardStrBuilder.Append(" ");
                foreach (var word in backwardList)
                {
                    forwardStrBuilder.Append(word);
                    forwardStrBuilder.Append(" ");
                }
                for (var idx = 0; idx < backwardMargin - charCount - 1; idx++)
                {
                    forwardStrBuilder.Append(" ");
                }
                return forwardStrBuilder.ToString();
            }

            private bool GetWords(int idx, int charLimit, List<string> wordList, ref int charCount)
            {
                var word = _wordList[idx];
                var count = charCount + word.Length + 1;
                if (count > charLimit)
                    return true;
                charCount = count;
                wordList.Add(word);
                return false;
            }
        }

        public class KWICSample
        {
             public static string Text = "The last question was asked for the first time, half in jest, on May 21, 2061, at a time when humanity first stepped into the light. The question came about as a result of a five dollar bet over highballs, and it happened this way: Alexander Adell and Bertram Lupov were two of the faithful attendants of Multivac. As well as any human beings could, they knew what lay behind the cold, clicking, flashing face -- miles and miles of face -- of that giant computer. They had at least a vague notion of the general plan of relays and circuits that had long since grown past the point where any single human could possibly have a firm grasp of the whole. Multivac was self - adjusting and self-correcting.It had to be, for nothing human could adjust and correct it quickly enough or even adequately enough-- so Adell and Lupov attended the monstrous giant only lightly and superficially, yet as well as any men could.They fed it data, adjusted question to its needs and translated the answers that were issued.Certainly they, and all others like them, were fully entitled to share In the glory that was Multivac'.";
        }

        public class DelimiterChars
        {
            public static readonly char[] Basic = { ' ', ',', '.', ':', '\t', '\"', '\'', '\r', '\n' };
        }
        

        public class WordSet
        {
            public string Word { get; set; }
            public int Index { get; set; } 
        }
        
    }
}
