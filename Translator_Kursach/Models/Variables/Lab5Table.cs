using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Translator.Models.Variables
{
    public class Lab5Table
    {
        public List<string> _Stack { get; set; }
        public List<string> _Lexems { get; set; }
        public string Relation { get; set; }
        public string First10Stack { get; set; }
        public string LexemString { get; set; }
        public Lab5Table(List<string> stack, List<string> lexems, int relation)
        {
            _Stack = stack;
            _Lexems = lexems;
            if (relation == 1)
                Relation = "<";
            else if (relation == 2)
                Relation = "=";
            else if (relation == 3)
                Relation = ">";
            First10Stack = GetFirst10Stack();
            LexemString = GetStringLexem();
        }
        public string GetStringStack()
        {
            string result = "";
            for (int i = 0; i < _Stack.Count; i++)
                result += _Stack[i] + " ";
            return result;
        }
        public string GetStringLexem()
        {
            string result = "";
            for (int i = 0; i < _Lexems.Count; i++)
                result += _Lexems[i] + " ";
            return result;
        }
        public string GetFirst10Stack()
        {
            string result = "... ";
            int endIndex;
            if (_Stack.Count >= 6)
            {
                endIndex = 6;
            }
            else
                endIndex = _Stack.Count;
            for(int i=_Stack.Count-endIndex; i<_Stack.Count;i++)
            {
                result += _Stack[i]+" ";
            }
            return result;
        }
    }
}