using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Translator.Models.Variables;

namespace Translator.Models
{
    public interface ILexemAnalyzator
    {
        void DoAnalyze(out string param);
        OutTables GetOutTables();
    }
    public class LexemAnalyzator:ILexemAnalyzator
    {
        private readonly string _sourceText; //текст кода програми
        private readonly List<Lexem> _inputLexem; //таблиця вхідних лексем, задається в базі даних
        private OutTables _outTables;
        public LexemAnalyzator(List<Lexem> inputLexem, string sourceText)
        {
            _sourceText = sourceText;
            _inputLexem = inputLexem;
            _outTables = new OutTables(new List<OutLexem>(), new List<Identify>(), new List<Const>());
        }

        public void DoAnalyze(out string analyzeResult)
        {
            ResetTables();
            analyzeResult = "";
            if (_sourceText == "")
                return;

            string[] CodeRows = _sourceText.Split('\n');//ділимо на масив рядків
            for(int i=0; i<CodeRows.Count(); i++)
            {
                CodeRows[i] = ValidationStr(CodeRows[i]);// видаляємо службові символи (\r)
                if (CodeRows[i]!="")
                {
                    CodeRows[i]=InsertWhiteSpace(CodeRows[i]);
                    if (!LexemParser(CodeRows[i],i+1, out analyzeResult))
                        break;
                    _outTables._OutLexem.Add(new OutLexem(_outTables._OutLexem.Count()+1, i + 1, "¶", 15, null));
                }
            }
            if (analyzeResult == "")
            {
                _outTables._OutLexem.RemoveAt(_outTables._OutLexem.Count() - 1);
                analyzeResult = "Successful Done";
            }
        }

        private string ValidationStr(string str)
        {
            if (str[str.Length-1]=='\r')
            {
                string temp = "";
                for (int i = 0; i < str.Length - 1; i++)
                    temp += str[i];
                return temp;
            }
            else { return str; }
        }

        private string InsertWhiteSpace(string row)//додаємо пробіли біля сепараторів
        {
            string temp = "";
            for (int i = 0; i < row.Length; i++)
            {
                if (row[i] == '!') 
                {
                    if (i < row.Length - 1)
                    {
                        if (row[i + 1] == '=')//перевіряємо на [ != ]
                        {
                            temp = temp + " " + row[i] + row[i + 1] + " ";
                            i++;
                        }
                        else
                        {
                            row += " " + row[i] + " ";
                        }
                    }
                    else
                    {
                        row += " " + row[i];
                    }
                }
                else
                {
                    var obj = _inputLexem.Find(o => o.Token == row[i].ToString());//шукаємо символ в таблиці вхідних лексем
                    if (obj != null)
                    {
                        if (obj.Separator)//якщо сепаратор [ + - * / < > = { } ( ) , ]
                        {
                            if (obj.Compatible.Count()!=0) // якщо може з'єднуватися з іншими смволами [ < > =]
                            {
                                if (i < row.Length - 1)
                                {
                                    if (obj.Compatible.Where(o => o.CanJoin == row[i + 1].ToString()).Count()!=0)//якщо може зєднуватися з row[i+1]
                                    {
                                        temp = temp + " " + row[i] + row[i + 1] + " ";
                                        i++;
                                    }
                                    else
                                    {
                                        temp = temp + " " + row[i] + " ";
                                    }
                                }
                                else
                                {
                                    temp = temp + " " + row[i];
                                }
                            }
                            else
                            {
                                temp = temp + " " + row[i] + " ";
                            }
                        }
                    }
                    else
                    {
                        temp += row[i];
                    }
                }
            }
            temp = DeleteExcessiveSpace(temp);
            return temp;
        }

        private string DeleteExcessiveSpace(string str)//видалення зайвих пробілів
        {
            string temp = "";
            for(int i=0; i<str.Length; i++)
            {
                if (str[i] != ' ')
                    temp += str[i];
                else
                {
                    temp += str[i];
                    while (i + 1 < str.Length - 1 && str[i + 1] == ' ')
                        i++;
                }
            }
            string result = "";
            int startIndex = 0;
            int endIndex = temp.Length-1;
            while (temp[startIndex] == ' ' && startIndex<endIndex)
                startIndex++;
            while (temp[endIndex] == ' ' && endIndex>startIndex)
                endIndex--;
            for (int i = startIndex; i <= endIndex; i++)
                result += temp[i];
            return result;
        }

        private bool LexemParser(string codeRow, int rowNumber, out string analyzeResult)
        {
            string lastType = ""; //зміння для зберігання останнього об'явленого типу даних
            analyzeResult = "";
            string[] Tokens = codeRow.Split(' ');//ділимо по пробілу
            foreach(string token in Tokens)
            {
                if (token!="")
                {
                    var obj = _inputLexem.Find(o => o.Token == token);
                    if (obj!=null)
                    {
                        _outTables._OutLexem.Add(new OutLexem(_outTables._OutLexem.Count() + 1, rowNumber, token, obj.Id, null));
                        if (obj.Id==3 || obj.Id==4 || obj.Id==1)
                        {
                            lastType = obj.Token;
                        }
                    }
                    else if ((token[0]>=65 && token[0]<=90) || ((token[0]>=97 && token[0]<=122) || (token[0]==95))) // A-Z..a-z.._
                    {
                        if (IsIdentify(token))
                        {
                            if (_outTables._OutLexem.FindIndex(o => o.Token == "{") == -1)//якщо ми ще в розділі оголошення змінних
                            {
                                if (_outTables._IdentifyTable.FindIndex(i=>i.Token==token)!=-1)//якщо таблиця ідентифікаторів вже містить даний токен
                                {
                                    analyzeResult += "Error! Duplicate identify! " + token + " | row:" + rowNumber;
                                }
                                else
                                {
                                    _outTables._IdentifyTable.Add(new Identify(_outTables._IdentifyTable.Count() + 1, token, lastType));
                                    _outTables._OutLexem.Add(new OutLexem(_outTables._OutLexem.Count() + 1, rowNumber, token, 34, _outTables._IdentifyTable.Count()));
                                }
                            }
                            else
                            {
                                int index = _outTables._IdentifyTable.FindIndex(o => o.Token == token);//індекс ідентифікатора в таблиці ідентифікаторів
                                if (index==-1)
                                {
                                    analyzeResult+= "Error! Undefiend Identify! " + token + " | row:" + rowNumber;
                                    return false;
                                }
                                else
                                {
                                    _outTables._OutLexem.Add(new OutLexem(_outTables._OutLexem.Count() + 1, rowNumber, token, 34, index + 1));
                                }
                            }
                        }
                    }
                    else if (IsConst(token))
                    {
                        int index = _outTables._ConstTable.FindIndex(o => o.Token == token);//індекс константи
                        if (index==-1)
                        {
                            _outTables._ConstTable.Add(new Const(_outTables._ConstTable.Count() + 1, token));
                            _outTables._OutLexem.Add(new OutLexem(_outTables._OutLexem.Count() + 1, rowNumber, token, 35, _outTables._ConstTable.Count()));
                        }
                        else
                        {
                            _outTables._OutLexem.Add(new OutLexem(_outTables._OutLexem.Count() + 1, rowNumber, token, 35, index+1));
                        }
                    }
                    else
                    {
                        analyzeResult += "Error! Undefiend token " + token + " |row: " + rowNumber;
                    }
                }
            }
            return true;
        }

        private bool IsIdentify(string token)
        {
            if (token.Length > 1)
                for (int i = 1; i < token.Length; i++)
                {
                    if (!((token[i] >= 65 && token[i] <= 90) || ((token[i] >= 97 && token[i] <= 122) || ((token[i] == 95) || (token[i] >= 48 && token[i] <= 57)))))//A-Z..a-z.._..1-9
                        return false;
                }
            return true;
        }
        private bool IsConst(string token)
        {
            int k = 0;//кількість входжень точки в константу
            for(int i=0; i<token.Length;i++)
            {
                if (token[i] >= 48 && token[i] <= 57)
                    continue;
                else if (token[i] == 46)
                {
                    k++;
                    if (k > 1)
                        return false;
                }
                else return false;
            }
            return true;
        }

        public OutTables GetOutTables()
        {
            return _outTables;
        }
        private void ResetTables()
        {
            _outTables._ConstTable.Clear();
            _outTables._IdentifyTable.Clear();
            _outTables._OutLexem.Clear();
        }
    }
}