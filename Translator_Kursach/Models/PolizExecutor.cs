using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Translator.Models.Variables;

namespace Translator.Models
{
    public class PolizExecutor
    {
        private readonly List<Lab7WorkItem> _Poliz;
        private readonly List<PolizLabel> _PolizLabels;
        public List<Lab8Identify> _IdentifyTable;
        public Stack<Lab7WorkItem> _Stack;
        public string _Status;
        public string _Errors;
        public int _Position;
        public object _OutParam;

        public PolizExecutor(List<Lab7WorkItem> poliz, List<PolizLabel> polizLabel, List<Identify> identifyTable)
        {
            _Poliz = poliz;
            _PolizLabels = polizLabel;
            _Status = "";
            _Errors = "";
            _Position = 0;
            _Stack = new Stack<Lab7WorkItem>();
            _IdentifyTable = new List<Lab8Identify>();
            for (int i = 0; i < identifyTable.Count; i++)
                _IdentifyTable.Add(new Lab8Identify(identifyTable[i].Token, identifyTable[i].Type, null));
        }


        public void GoNextStep()
        {
            _Status = "";
            if (_Position>_Poliz.Count-1)
            {
                _Status = "Successful Done";
                return;
            }
            if (_Poliz[_Position].Type == "label" && _Poliz[_Position].Token[_Poliz[_Position].Token.Length - 1] == ':')
            {
                _Position++;
            }
            else if (_Poliz[_Position].Type=="IDN" || _Poliz[_Position].Type=="CST" || _Poliz[_Position].Type=="label")
            {
                _Stack.Push(_Poliz[_Position]);
                _Position++;
            }    
            else if (_Poliz[_Position].Type=="Operation")
            {
                if (_Poliz[_Position].Token==">>")
                {
                    _Status = "Input ";
                    if (_Stack.Count>0)
                    {
                        string idn = _Stack.Peek().Token;
                        var obj = _IdentifyTable.Find(o => o.Token == idn);
                        if (obj != null)
                        {
                            _Status += obj.Type;
                            return;
                        }
                        else
                            _Errors += "identify not found";
                        return;
                    }
                }
                else if (_Poliz[_Position].Token=="<<")
                {
                    _Status = "Output ";
                    if (_Stack.Count > 0)
                    {
                        string idn = _Stack.Peek().Token;
                        var obj = _IdentifyTable.Find(o => o.Token == idn);
                        if (obj != null)
                        {
                            _Status += obj.Type;
                            _OutParam = obj.Value;
                            return;
                        }
                        else
                            _Errors += "identify not found";
                        return;
                    }
                }
                else if (_Poliz[_Position].Token=="+" || _Poliz[_Position].Token == "/" || _Poliz[_Position].Token == "*"|| _Poliz[_Position].Token == "-")
                {
                    if (_Stack.Count > 1)
                    {
                        var item1 = _Stack.Pop();
                        var item2 = _Stack.Pop();

                        double val2 = GetItemValue(item1);
                        double val1 = GetItemValue(item2);
                        string operation = _Poliz[_Position].Token;
                        double res = 0;
                        if (operation=="+")
                            res = val1 + val2;
                        else if (operation == "-")
                            res = val1 - val2;
                        else if (operation == "*")
                            res = val1 * val2;
                        else if (operation == "/")
                            res = val1 / val2;
                        _Stack.Push(new Lab7WorkItem(res.ToString(), "CST"));
                        _Position++;
                    }
                    else
                        throw new InvalidOperationException();
                }
                else if (_Poliz[_Position].Token=="@")
                {
                    var item1 = _Stack.Pop();
                    double val1 = GetItemValue(item1);
                    double res = -val1;
                    _Stack.Push(new Lab7WorkItem(res.ToString(), "CST"));
                    _Position++;
                }
                else if (_Poliz[_Position].Token==">"||_Poliz[_Position].Token=="<" || _Poliz[_Position].Token == "<=" || _Poliz[_Position].Token == ">=" || _Poliz[_Position].Token == "==")
                {
                    var item1 = _Stack.Pop();
                    var item2 = _Stack.Pop();

                    double val2 = GetItemValue(item1);
                    double val1 = GetItemValue(item2);

                    string operation = _Poliz[_Position].Token;

                    bool res = false;

                    if (operation == ">")
                        res = val1 > val2;
                    else if (operation == "<")
                        res = val1 < val2;
                    else if (operation == "<=")
                        res = val1 <= val2;
                    else if (operation == ">=")
                        res = val1 >= val2;
                    else if (operation == "==")
                        res = val1 == val2;
                    _Stack.Push(new Lab7WorkItem(res.ToString(), "CST"));
                    _Position++;
                }
                else if (_Poliz[_Position].Token=="=")
                {
                    var item1 = _Stack.Pop();
                    var item2 = _Stack.Pop();
                    var obj = _IdentifyTable.Find(o => o.Token == item2.Token);

                    double val = GetItemValue(item1);

                    if (obj.Type == "int")
                    {
                        int res = (int)val;
                        obj.Value = (object)res;
                    }
                    else if (obj.Type == "double")
                    {
                        obj.Value = (object)val;
                    }
                    else
                        throw new InvalidOperationException();
                    _Position++;
                }
                else if (_Poliz[_Position].Token=="UPL")
                {
                    var item1 = _Stack.Pop();
                    var item2 = _Stack.Pop();

                    if (item2.Token == "False")
                    {
                        var obj = _PolizLabels.Find(o => o.Label == item1.Token);
                        _Position = obj.Position;
                    }
                    else
                        _Position++;
                }
                else if (_Poliz[_Position].Token=="BP")
                {
                    var item1 = _Stack.Pop();
                    var obj = _PolizLabels.Find(o => o.Label == item1.Token);
                    _Position = obj.Position;
                }
                else if (_Poliz[_Position].Token=="and")
                {
                    var item1 = _Stack.Pop();
                    var item2 = _Stack.Pop();
                    if (item1.Token == "True" && item2.Token == "True")
                        _Stack.Push(new Lab7WorkItem("True", "CST"));
                    else
                        _Stack.Push(new Lab7WorkItem("False", "CST"));

                    _Position++;
                }
                else if (_Poliz[_Position].Token=="or")
                {
                    var item1 = _Stack.Pop();
                    var item2 = _Stack.Pop();
                    if (item1.Token == "True" || item2.Token == "True")
                        _Stack.Push(new Lab7WorkItem("True", "CST"));
                    else
                        _Stack.Push(new Lab7WorkItem("False", "CST"));
                    _Position++;
                }
                else if (_Poliz[_Position].Token=="not")
                {
                    var item1 = _Stack.Pop();
                    if (item1.Token == "True")
                        _Stack.Push(new Lab7WorkItem("False", "CST"));
                    else if (item1.Token == "False")
                        _Stack.Push(new Lab7WorkItem("True", "CST"));
                    else
                        throw new InvalidOperationException("Not, can be used with logical value");
                    _Position++;
                }
                else
                {
                    throw new InvalidOperationException("undefiend operation");
                }
            }
        }

        private double GetItemValue(Lab7WorkItem item)
        {
            double result = 0;
            if (item.Type == "CST")
            {
                if (Double.TryParse(item.Token, out result))
                    return result;
                else
                    throw new ArgumentException();
            }
            else if (item.Type == "IDN")
            {
                var idn = _IdentifyTable.Find(o => o.Token == item.Token);
                if (idn.Type == "int")
                    if (idn.Value != null)
                        result = (double)(int)idn.Value;
                    else
                        throw new NullReferenceException("The identify " + idn.Token + " didn't initialized");
                else if (idn.Type == "double")
                    if (idn.Value != null)
                        result = (double)idn.Value;
                    else
                        throw new NullReferenceException("The identify " + idn.Token + " didn't initialized");
                else
                    throw new ArgumentException();
            }
            else
                throw new ArgumentException();
            return result;
        }
    }

}