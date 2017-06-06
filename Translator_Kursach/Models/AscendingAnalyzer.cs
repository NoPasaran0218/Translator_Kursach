using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Translator.Models
{
    public interface IAscendingAnalyzer
    {
        void SetRelations();
        string[,] GetRelationMatrix();
        List<string> GetAllTerm();
        Dictionary<string, List<List<string>>> GetGrammar();
    }

    public class AscendingAnalyzer:IAscendingAnalyzer
    {
        Dictionary<string, List<List<string>>> _Grammar = new Dictionary<string, List<List<string>>>();
        Dictionary<string, List<string>> _lastPlus = new Dictionary<string, List<string>>();
        Dictionary<string, List<string>> _firstPlus = new Dictionary<string, List<string>>();
        HashSet<string> _allTerms = new HashSet<string>();//все разом
        HashSet<string> _unTerm = new HashSet<string>();//лише нетермінали, тобто ті що починаються з "<" і закінчуються ">"
        string[,] _relationMatrix;
        string _grammarText;
        public AscendingAnalyzer(string grammarText)
        {
            _grammarText = grammarText;
        }

        public void SetRelations()
        {
            GetGrammarAndTerms();
            GetEqualySign();
            GetFirstPlus();
            GetLastPlus();
            SetLessSign2();
            SetMoreSign();
            SetSharpRelation();//додавання # 
        }

        #region lab4
        private void GetGrammarAndTerms()//заповнюємо _Grammar, _allTerms, _unTerm
        {
            string[] stroki = _grammarText.Split('\n');

            for (int i = 0; i < stroki.Count(); i++)
                if (stroki[i] != "")
                    if (stroki[i][stroki[i].Length - 1] == '\r')
                        stroki[i] = stroki[i].Substring(0, stroki[i].Length - 1); //видалення \r

            for(int i=0; i<stroki.Count(); i++)
            {
                if (stroki[i] != "")
                {
                    string leftPart = GetLeftPart(stroki[i]);//до ::=
                    string rightPart = GetRightPart(stroki[i]);//після ::=
                    _allTerms.Add(leftPart);
                    _unTerm.Add(leftPart);
                    _Grammar[leftPart] = new List<List<string>>();
                    List<string> tempList; //тимчасова змінна для зберігання всіх термів частини А::=ВС|ASD tempList = BC a потім ASD
                    string[] parts = rightPart.Split('|');
                    for (int j = 0; j < parts.Count(); j++)
                    {
                        string[] terms = parts[j].Split(' ');
                        tempList = terms.ToList();
                        _Grammar[leftPart].Add(tempList);
                        foreach (string term in terms)
                            _allTerms.Add(term);
                    }
                }
            }
        }

        private void GetFirstPlus()
        {
            foreach(string unTerm in _unTerm)
            {
                HashSet<string> tempFirstPlus = new HashSet<string>();
                List<List<string>> rightParts = _Grammar[unTerm];//права частина граматики
                HashSet<string> tempUnTerm = new HashSet<string>();

                for(int i=0; i<rightParts.Count; i++)
                {
                    string rightFirst = rightParts[i].First();
                    tempFirstPlus.Add(rightFirst);
                    if (_unTerm.Contains(rightFirst))
                        tempUnTerm.Add(rightFirst);
                }

                int iter = 0; //кількість знайдений firstPlus для елементів з tempUnTerm
                while (iter < tempUnTerm.Count)
                {
                    string currentUnTerm = tempUnTerm.ElementAt(iter);
                    rightParts = _Grammar[currentUnTerm];
                    for(int i=0; i<rightParts.Count; i++)
                    {
                        string rightFirst = rightParts[i].First();
                        tempFirstPlus.Add(rightFirst);
                        if (_unTerm.Contains(rightFirst))
                            tempUnTerm.Add(rightFirst);
                    }
                    iter++;
                }
                _firstPlus[unTerm] = new List<string>();
                _firstPlus[unTerm].AddRange(tempFirstPlus);
            }
        }
        public void GetLastPlus()
        {
            foreach (string unTerm in _unTerm)
            {
                HashSet<string> tempLastPlus = new HashSet<string>();
                List<List<string>> rightParts = _Grammar[unTerm];//права частина граматики
                HashSet<string> tempUnTerm = new HashSet<string>();

                for (int i = 0; i < rightParts.Count; i++)
                {
                    string rightLast = rightParts[i].Last();
                    tempLastPlus.Add(rightLast);
                    if (_unTerm.Contains(rightLast))
                        tempUnTerm.Add(rightLast);
                }

                int iter = 0; //кількість знайдений lastPlus для елементів з tempUnTerm
                while (iter < tempUnTerm.Count)
                {
                    string currentUnTerm = tempUnTerm.ElementAt(iter);
                    rightParts = _Grammar[currentUnTerm];
                    for (int i = 0; i < rightParts.Count; i++)
                    {
                        string rightLast = rightParts[i].Last();
                        tempLastPlus.Add(rightLast);
                        if (_unTerm.Contains(rightLast))
                            tempUnTerm.Add(rightLast);
                    }
                    iter++;
                }
                _lastPlus[unTerm] = new List<string>();
                _lastPlus[unTerm].AddRange(tempLastPlus);
            }
        }

        private string GetLeftPart(string input)
        {
            int index = input.IndexOf("::=");
            string temp = "";
            if (index > -1)
                for (int i = 0; i < index; i++)
                    temp += input[i];
            return temp;
        }

        private string GetRightPart(string input)
        {
            int index = input.IndexOf("::=");
            string temp = "";
            if (index > -1)
                for (int i = index + 3; i < input.Length; i++)
                    temp += input[i];
            return temp;
        }

        private void GetEqualySign()
        {
            _relationMatrix = GetClearMatrix(_allTerms.Count);
            List<string> tempList = _allTerms.ToList(); // тимчасова змінна
            foreach(var d in _Grammar)
            {
                for(int i=0; i<d.Value.Count; i++)
                {
                    for(int j=0; j<d.Value[i].Count-1;j++)
                    {
                        int indexI = tempList.IndexOf(d.Value[i][j]);
                        int indexJ = tempList.IndexOf(d.Value[i][j + 1]);
                        _relationMatrix[indexI,indexJ] = "=";
                    }
                }
            }
        }

        private string[,] GetClearMatrix(int Size)
        {
            string[,] result = new string[Size, Size];
            for (int i = 0; i < Size; i++)
                for (int j = 0; j < Size; j++)
                    result[i, j] = "";
            return result;
        }

        private string[,] GetFirstPlusMatrix()
        {
            string[,] result = GetClearMatrix(_allTerms.Count);
            List<string> tempAllTerms = _allTerms.ToList();
            foreach(string unTerm in _unTerm)
            {
                int indexI = tempAllTerms.IndexOf(unTerm);
                List<string> firstPlus = _firstPlus[unTerm];
                for(int i=0; i<firstPlus.Count; i++)
                {
                    int indexJ = tempAllTerms.IndexOf(firstPlus[i]);
                    result[indexI, indexJ] += "F";
                }
            }
            return result;
        }

        private void SetLessSign()
        {
            string [,] firstPlusMatrix = GetFirstPlusMatrix();
            int size = _allTerms.Count;
            for(int i=0; i<size; i++)
            {
                for(int j=0;j<size;j++)
                {
                    for(int k=0;k<size;k++)
                    {
                        if (_relationMatrix[i, k] == "=" && firstPlusMatrix[k, j] == "F")
                            _relationMatrix[i, j] += "<";
                    }
                }
            }
            return;
        }

        private void SetLessSign2()
        {
            List<string> tempAllTerm = _allTerms.ToList();
            int size = tempAllTerm.Count;
            foreach( var item in _allTerms)
            {
                if (!_unTerm.Contains(item))
                {
                    int indexI = tempAllTerm.IndexOf(item);
                    for (int j=0;j<size;j++)
                    {
                        if (_relationMatrix[indexI,j]=="=" && _unTerm.Contains(tempAllTerm[j]))
                        {
                            string unTerm = tempAllTerm[j];
                            for(int k=0; k<_firstPlus[unTerm].Count;k++)
                            {
                                int indexJ = tempAllTerm.IndexOf(_firstPlus[unTerm][k]);
                                if (_relationMatrix[indexI, indexJ] != "<")
                                    _relationMatrix[indexI, indexJ] += "<";
                            }
                        }
                    }
                }
            }
        }

        private string [,] GetLastPlusMatrix()
        {
            string[,] result = GetClearMatrix(_allTerms.Count);
            List<string> tempAllTerms = _allTerms.ToList();
            foreach (string unTerm in _unTerm)
            {
                int indexI = tempAllTerms.IndexOf(unTerm);
                List<string> lastPlus = _lastPlus[unTerm];
                for (int i = 0; i < lastPlus.Count; i++)
                {
                    int indexJ = tempAllTerms.IndexOf(lastPlus[i]);
                    result[indexI, indexJ] += "L";
                }
            }
            return result;
        }

        private void SetMoreSign()
        {
            int size = _allTerms.Count;
            List<string> tempAllTerm = _allTerms.ToList();
            foreach(string unTerm in _unTerm)
            {
                int indexI = tempAllTerm.IndexOf(unTerm);
                for(int j=0;j<size; j++)
                    if(_relationMatrix[indexI,j]=="=")
                    {
                        string S = tempAllTerm[j];
                        List<string> lastPlusR = _lastPlus[unTerm];
                        if (_unTerm.Contains(S)) //якщо S нетермінал то Last+(unTerm)>First+(S)
                        {
                            List<string> firstPlusS = _firstPlus[S];
                            InsertMoreSignInMatrix(lastPlusR, firstPlusS);
                        }
                        else
                        {
                            InsertMoreSignInMatrix(lastPlusR, S);
                        }
                    }
            }
        }

        private void InsertMoreSignInMatrix(List<string> lastPlusR, List<string> firstPlusS)
        {
            List<string> tempAllTerm = _allTerms.ToList();
            foreach (string lastPlusRItem in lastPlusR)
            {
                int indexI = tempAllTerm.IndexOf(lastPlusRItem);
                foreach(string firstPlusSItem in firstPlusS)
                {
                    int indexJ = tempAllTerm.IndexOf(firstPlusSItem);
                    if (_relationMatrix[indexI, indexJ] != ">")
                        _relationMatrix[indexI, indexJ] += ">";
                }
            }
        }
        private void InsertMoreSignInMatrix(List<string> lastPlusR, string S)
        {
            List<string> tempAllTerm = _allTerms.ToList();
            int indexJ = tempAllTerm.IndexOf(S);
            foreach (string lastPlusRItem in lastPlusR)
            {
                int indexI = tempAllTerm.IndexOf(lastPlusRItem);
                if (_relationMatrix[indexI, indexJ] != ">")
                    _relationMatrix[indexI, indexJ] += ">";
            }
        }

        public string[,] GetRelationMatrix() => _relationMatrix;
        public List<string> GetAllTerm() => _allTerms.ToList();

        public void SetSharpRelation()
        {
            _allTerms.Add("#");
            string[,] result = GetClearMatrix(_allTerms.Count);
            for (int i = 0; i < _allTerms.Count - 1; i++)
            {
                for (int j = 0; j < _allTerms.Count - 1; j++)
                    result[i, j] = _relationMatrix[i, j];
                result[i, _allTerms.Count - 1] = ">";
                result[_allTerms.Count - 1, i] = "<";
            }
            _relationMatrix = result;
        }

        public Dictionary<string, List<List<string>>> GetGrammar() => _Grammar;
        #endregion


    }
}