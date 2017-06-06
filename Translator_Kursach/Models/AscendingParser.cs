using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Translator.Models.Variables;

namespace Translator.Models
{
    public class AscendingParser
    {
        Dictionary<string, List<List<string>>> _Grammar = new Dictionary<string, List<List<string>>>();
        List<string> _allTerms = new List<string>();//все разом
        string[,] _relationMatrixVisual;
        List<OutLexem> _inputLexem;
        int[,] _relationMatrix;
        bool ind = false;
        Stack<string> _Stack = new Stack<string>();
        public List<Lab5Table> ResultTable = new List<Lab5Table>();

        public AscendingParser(Dictionary<string, List<List<string>>> Grammar, List<string> allTerms, string[,] relationMatrix, List<OutLexem> inputLexem )
        {
            _Grammar = Grammar;
            _allTerms = allTerms;
            _relationMatrixVisual = relationMatrix;
            SetRelationMatrix();
            _inputLexem = inputLexem;
        }

        private void SetRelationMatrix()//перевод матриці стрінгів в інт де "<"=1, "="=2, ">"=3
        {
            int size = _allTerms.Count;
            _relationMatrix = new int[size, size];
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    if (_relationMatrixVisual[i, j] != "")
                        if (_relationMatrixVisual[i, j] == "<")
                            _relationMatrix[i, j] = 1;
                        else if (_relationMatrixVisual[i, j] == "=")
                            _relationMatrix[i, j] = 2;
                        else if (_relationMatrixVisual[i, j] == ">")
                            _relationMatrix[i, j] = 3;
                        else _relationMatrix[i, j] = 0;
        }

        public string Parse()
        {
            _inputLexem.Add(new OutLexem(_inputLexem.Count+1, 1000,"#",-1,null));
            _Stack.Push("#");
            while (_inputLexem.Count>0)
            {
                if (ind)
                {
                    ResultTable.Add(new Lab5Table(GetStackList(_Stack), GetLexemStringList(_inputLexem), 3));
                    break;
                }
                if (_inputLexem[0].Token == "#")
                    ind = true;
                string stackHead = _Stack.Peek();
                string firstLexem = _inputLexem.First().Token;
                int relation = GetRelation(stackHead, firstLexem);//отримуємо відношення між верхнім елементом стека і наступним елементом вхідного рядка
                ResultTable.Add(new Lab5Table(GetStackList(_Stack),GetLexemStringList(_inputLexem), relation));
                if (relation==1 || relation==2)
                {
                    _Stack.Push(_inputLexem.First().Token);
                    _inputLexem.RemoveAt(0);
                }
                else if (relation==3)
                {
                    List<string> temp = new List<string>();
                    temp.Add(_Stack.Pop());
                    while(_Stack.Count>0)
                    {
                        if (GetRelation(_Stack.Peek(), temp[0]) == 1)
                            break;
                        else
                        {
                            temp.Insert(0, _Stack.Pop());
                        }
                    }

                    string unTerm = GetAppropriateUnderm(temp);
                    if (unTerm == "")
                        return "Error: "+ temp.Last();
                    _Stack.Push(unTerm);
                }
                else { return "Error! row: "+_inputLexem.First().Row+" | Token => "+ _inputLexem.First().Token; }
            }
            if (_Stack.Peek() == "<прогр>")
                return "Successful Done";
            else
                return "Помилка виконання програми";
        }

        private List<string> GetStackList(Stack<string> _Stack)
        {
            List<string> result = new List<string>();
            for (int i = 0; i < _Stack.Count; i++)
                result.Insert(0,_Stack.ElementAt(i));
            return result;
        }

        private List<string> GetLexemStringList(List<OutLexem> _inputLexem)
        {
            List<string> result = new List<string>();
            for (int i = 0; i < _inputLexem.Count; i++)
                result.Add(_inputLexem[i].Token);
            return result;
        }

        private string GetAppropriateUnderm(List<string> rightPart)//по заданій правій частині повертає відповідний unTerm <правило>
        {
            foreach (var rule in _Grammar)
            {
                for(int i=0; i<rule.Value.Count;i++)
                {
                    if (IsEqual(rule.Value[i], rightPart))
                        return rule.Key;
                }
            }
            return "";
        }

        private bool IsEqual(List<string> list1, List<string> list2)
        {
            if (list1.Count==list2.Count)
            {
                for (int i = 0; i < list1.Count; i++)
                    if (list1[i] != list2[i])
                        return false;
                return true;
            }
            else
            {
                return false;
            }
        }

        private int GetRelation(string term1, string term2)//отримуємо відношення між двома термами
        {
            int indexI = _allTerms.IndexOf(term1);
            int indexJ = _allTerms.IndexOf(term2);
            if (indexI!=-1 && indexJ!=-1)
            {
                return _relationMatrix[indexI, indexJ];
            }
            else { throw new Exception("nevidomyi term"); }
        }
    }
}