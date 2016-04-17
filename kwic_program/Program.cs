﻿using System;
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

            var kwic = new KWIC(text);
            var results = kwic.Concordance("last");

            foreach (var result in results)
            {
                Console.WriteLine(result);
            }

        }

        public class KWIC
        {
            private readonly char[] _delimiterChars;
            public string OrginString { get; private set; }
            
            public int ForwardCount
            {
                get { return _forwardCount; }
                set
                {
                    if (value < 0)
                        throw new ArgumentOutOfRangeException("need to natural number");
                    _forwardCount = value;
                }
            }
            private int _forwardCount = 0;

            public int BackwardCount
            {
                get { return _backwardCount; }
                set
                {
                    if (value < 0)
                        throw new ArgumentOutOfRangeException("need to natural number");
                    _backwardCount = value;
                }
            }
            private int _backwardCount = 0;

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

                BackwardCount = ForwardCount = 30;
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

            public List<string> Concordance(string target)
            {
                var searchedWords = 
                    _wordSetList.Where(q => q.Word == target).ToList();

                return searchedWords.Count == 0 ?
                    new List<string>() : 
                    searchedWords.Select(PrintLine).ToList();
            }

            private string PrintLine(WordSet wordSet)
            {
                var forwardwords = BuildForwardWords(wordSet, ForwardCount);
                var backwardWords = BuildBackwardWords(wordSet, BackwardCount);
                
                var line = new StringBuilder();
                line.Append(forwardwords);
                line.Append(wordSet.Word);
                line.Append(backwardWords);
                return line.ToString();
            }

            private string BuildForwardWords(WordSet wordSet, int forwardCount)
            {
                int charCount = 0;
                var forwardList = new List<string>();
                for (var idx = wordSet.Index - 1; idx > 0; idx--)
                {
                    if (GetWords(idx, forwardCount, forwardList, ref charCount)) break;
                }
                forwardList.Reverse();

                var forwardStrBuilder = new StringBuilder();
                for (var idx = 0; idx < forwardCount - charCount; idx++)
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

            private string BuildBackwardWords(WordSet wordSet, int backwardCount)
            {
                int charCount = 0;
                var forwardList = new List<string>();
                for (var idx = wordSet.Index + 1; idx < _wordList.Count; idx++)
                {
                    if (GetWords(idx, backwardCount, forwardList, ref charCount)) break;
                }

                var forwardStrBuilder = new StringBuilder();
                forwardStrBuilder.Append(" ");
                foreach (var word in forwardList)
                {
                    forwardStrBuilder.Append(word);
                    forwardStrBuilder.Append(" ");
                }
                for (var idx = 0; idx < backwardCount - charCount - 1; idx++)
                {
                    forwardStrBuilder.Append(" ");
                }
                return forwardStrBuilder.ToString();
            }

            private bool GetWords(int idx, int forward, List<string> forwardList, ref int charCount)
            {
                var word = _wordList[idx];
                var count = charCount + word.Length + 1;
                if (count > forward)
                    return true;
                charCount = count;
                forwardList.Add(word);
                return false;
            }
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