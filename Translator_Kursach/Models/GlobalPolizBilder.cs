using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Translator.Models.Variables;

namespace Translator.Models
{
    public class GlobalPolizBilder
    {
        public List<Lab7WorkItem> InputWorkItems;
        public List<PriorityTable> TablePriority;
        public Stack<Lab7WorkItem> _Stack = new Stack<Lab7WorkItem>();
        public List<Lab7WorkItem> Poliz;
        public List<PolizLabel> PolizLabels;
        bool isLoopNow = false;
        int loopEnd = 0;
        
        
        public GlobalPolizBilder(List<Lab7WorkItem> input)
        {
            InputWorkItems = input;
            TablePriority = new List<PriorityTable>();
            Poliz = new List<Lab7WorkItem>();
            PolizLabels = new List<PolizLabel>();
            TablePriority.Add(new PriorityTable("{", 0));
            TablePriority.Add(new PriorityTable("if", 1));
            TablePriority.Add(new PriorityTable("do", 1));
            TablePriority.Add(new PriorityTable("while", 1));
            TablePriority.Add(new PriorityTable("}", 1));
            TablePriority.Add(new PriorityTable("<<", 1));
            TablePriority.Add(new PriorityTable("cout", 1));
            TablePriority.Add(new PriorityTable("cin", 1));
            TablePriority.Add(new PriorityTable(">>", 1));
            TablePriority.Add(new PriorityTable("¶", 1));
            TablePriority.Add(new PriorityTable("(", 2));
            TablePriority.Add(new PriorityTable("then", 2));
            TablePriority.Add(new PriorityTable("enddo", 2));
            TablePriority.Add(new PriorityTable(")", 3));
            TablePriority.Add(new PriorityTable("or", 3));
            TablePriority.Add(new PriorityTable("=", 3));
            TablePriority.Add(new PriorityTable("and", 4));
            TablePriority.Add(new PriorityTable("not", 5));
            TablePriority.Add(new PriorityTable("<", 6));
            TablePriority.Add(new PriorityTable(">", 6));
            TablePriority.Add(new PriorityTable("<=", 6));
            TablePriority.Add(new PriorityTable(">=", 6));
            TablePriority.Add(new PriorityTable("==", 6));
            TablePriority.Add(new PriorityTable("!=", 6));
            TablePriority.Add(new PriorityTable("+", 7));
            TablePriority.Add(new PriorityTable("-", 7));
            TablePriority.Add(new PriorityTable("*", 8));
            TablePriority.Add(new PriorityTable("@", 8));
            TablePriority.Add(new PriorityTable("/", 8));
        }


        public void DoWork()
        {
            int i = 0;
            while(InputWorkItems.Count>0 && InputWorkItems[0].Token!="{")
            {
                InputWorkItems.RemoveAt(0);
            }

            while (InputWorkItems.Count>0)
            {
                if (InputWorkItems[0].Type=="IDN" || InputWorkItems[0].Type=="CST")
                {
                    Poliz.Add(InputWorkItems[0]);
                    InputWorkItems.RemoveAt(0);
                }
                else
                {
                    if (_Stack.Count == 0)
                    {
                        _Stack.Push(InputWorkItems[0]);
                        InputWorkItems.RemoveAt(0);
                    }
                    else
                    {
                        if (InputWorkItems[0].Token == "{")
                        {
                            _Stack.Push(InputWorkItems[0]);
                            InputWorkItems.RemoveAt(0);
                            continue;
                        }
                        else if (InputWorkItems[0].Token == "do")
                        {
                            string newLabel1 = "m" + (PolizLabels.Count + 1).ToString();
                            string newLabel2 = "m" + (PolizLabels.Count + 2).ToString();
                            PolizLabels.Add(new PolizLabel(newLabel1, PolizLabels.Count+1));
                            PolizLabels.Add(new PolizLabel(newLabel2, PolizLabels.Count+1));
                            _Stack.Push(new Lab7WorkItem(newLabel2, "label"));
                            _Stack.Push(new Lab7WorkItem(newLabel1, "label"));
                            _Stack.Push(InputWorkItems[0]);
                            InputWorkItems.RemoveAt(0);
                            isLoopNow = true;
                            loopEnd++;
                            continue;
                        }
                        else if (InputWorkItems[0].Token=="while")
                        {
                            if (_Stack.Count >= 2)
                            {
                                Poliz.Add(new Lab7WorkItem(_Stack.ElementAt(1).Token + ":", "label"));
                                var obj = PolizLabels.Find(o => o.Label == _Stack.ElementAt(1).Token);
                                if (obj != null)
                                    obj.Position = Poliz.Count;
                            }
                            InputWorkItems.RemoveAt(0);
                            continue;
                        }
                        else if (InputWorkItems[0].Token=="cout" || InputWorkItems[0].Token=="cin")
                        {
                            _Stack.Push(InputWorkItems[0]);
                            InputWorkItems.RemoveAt(0);
                            continue;
                        }
                        else if (InputWorkItems[0].Token=="(")
                        {
                            _Stack.Push(InputWorkItems[0]);
                            InputWorkItems.RemoveAt(0);
                            continue;
                        }
                        int priority1 = GetPriority(_Stack.Peek().Token);
                        int priority2 = GetPriority(InputWorkItems[0].Token);

                        while (priority1 >= priority2 && _Stack.Count > 0)
                        {
                            if (isLoopNow && _Stack.Peek().Token == "do")
                                break;
                            if (loopEnd > 0 && _Stack.Peek().Token == "do")
                                break;
                            if (_Stack.Peek().Token == "cout" || _Stack.Peek().Token == "cin")
                                if (InputWorkItems[0].Token == "<<" || InputWorkItems[0].Token == ">>")
                                {
                                    break;
                                }
                                else if (InputWorkItems[0].Token == "¶")
                                {
                                    _Stack.Pop();
                                    if (_Stack.Count > 0)
                                        priority1 = GetPriority(_Stack.Peek().Token);
                                    continue;
                                }
                            
                            if (_Stack.Peek().Token == "if")
                            {
                                Stack<Lab7WorkItem> temp = new Stack<Lab7WorkItem>();
                                temp.Push(_Stack.Pop());
                                while (_Stack.Peek().Type == "label")
                                    temp.Push(_Stack.Pop());
                                string lastLabel = temp.Peek().Token;
                                Poliz.Add(new Lab7WorkItem(temp.Pop().Token + ":", "label"));


                                PolizLabels.Find(o => o.Label == lastLabel).Position = Poliz.Count();

                                if (_Stack.Count>0)
                                    priority1 = GetPriority(_Stack.Peek().Token);
                            }
                            else
                            {
                                Poliz.Add(_Stack.Pop());
                                if (_Stack.Count>0)
                                priority1 = GetPriority(_Stack.Peek().Token);
                            }
                        }

                        if (InputWorkItems[0].Token == "¶")
                        {
                            InputWorkItems.RemoveAt(0);
                            if (isLoopNow)
                            {
                                while (_Stack.Peek().Token!="do")
                                {
                                    Poliz.Add(_Stack.Pop());
                                }
                                if (_Stack.Count > 2)
                                    Poliz.Add(_Stack.ElementAt(2));
                                Poliz.Add(new Lab7WorkItem("UPL", "Operation"));
                                isLoopNow = false;
                            }
                        }
                        else if (InputWorkItems[0].Token == ")")
                        {
                            while (_Stack.Peek().Token != "(")
                                Poliz.Add(_Stack.Pop());
                            InputWorkItems.RemoveAt(0);
                            _Stack.Pop();
                        }
                        else if (InputWorkItems[0].Token == "}")
                        {
                            while (_Stack.Peek().Token != "{")
                                if (_Stack.Peek().Token == "if")
                                {
                                    Stack<Lab7WorkItem> temp = new Stack<Lab7WorkItem>();
                                    temp.Push(_Stack.Pop());
                                    while (_Stack.Peek().Type == "label")
                                        temp.Push(_Stack.Pop());
                                    string lastLabel = temp.Peek().Token;
                                    Poliz.Add(new Lab7WorkItem(temp.Pop().Token + ":", "label"));


                                    PolizLabels.Find(o => o.Label == lastLabel).Position = Poliz.Count();
                                }
                            else
                                {
                                    Poliz.Add(_Stack.Pop());
                                }
                            InputWorkItems.RemoveAt(0);
                            _Stack.Pop();
                        }
                        else if (InputWorkItems[0].Token=="then")
                        {
                            while (_Stack.Peek().Token != "if")
                                Poliz.Add(_Stack.Pop());
                            Stack<Lab7WorkItem> temp = new Stack<Lab7WorkItem>();
                            temp.Push(_Stack.Pop());
                            while(_Stack.Peek().Type=="label")
                                temp.Push(_Stack.Pop());
                            string newLabel = "m" + (PolizLabels.Count + 1).ToString();
                            PolizLabels.Add(new PolizLabel(newLabel, PolizLabels.Count + 1));

                            _Stack.Push(new Lab7WorkItem(newLabel, "label"));
                            while (temp.Count > 0)
                                _Stack.Push(temp.Pop());

                            Poliz.Add(new Lab7WorkItem(newLabel, "label"));
                            Poliz.Add(new Lab7WorkItem("UPL", "Operation"));
                            InputWorkItems.RemoveAt(0);
                        }
                        else if (InputWorkItems[0].Token=="enddo")
                        {
                            while (_Stack.Peek().Token != "do")
                            {
                                if (_Stack.Peek().Token != "cout" && _Stack.Peek().Token != "cin")
                                    Poliz.Add(_Stack.Pop());
                                else
                                    _Stack.Pop();
                            }
                            Stack<Lab7WorkItem> temp = new Stack<Lab7WorkItem>();
                            _Stack.Pop();
                            if (_Stack.Count > 1)
                            {
                                Poliz.Add(_Stack.Pop());
                                Poliz.Add(new Lab7WorkItem("BP", "Operation"));
                            }
                            string lastLabel = _Stack.Pop().Token;
                            Poliz.Add(new Lab7WorkItem(lastLabel + ":", "label"));
                            InputWorkItems.RemoveAt(0);
                            loopEnd--;
                            PolizLabels.Find(o => o.Label == lastLabel).Position = Poliz.Count;
                        }
                        else
                        {
                            _Stack.Push(InputWorkItems[0]);
                            InputWorkItems.RemoveAt(0);
                        }
                    }
                }
            }
        }


        private int GetPriority(string token)
        {
            var obj = TablePriority.Find(o => o.Token == token);
            if (obj != null)
                return obj.Priority;
            else
                return -1;
        }

        public string GetPolizString()
        {
            string result = "";
            for (int i = 0; i < Poliz.Count; i++)
                result += Poliz[i].Token + " ";
            return result;
        }
    }
}