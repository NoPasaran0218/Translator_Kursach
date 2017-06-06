using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Translator.Models;
using Translator.Models.Variables;

namespace Translator_Kursach.Controllers
{
    public class HomeController : Controller
    {
        TranslatorContext dbContext = new TranslatorContext();
        static Dictionary<string, List<List<string>>> _Grammar;
        static ILexemAnalyzator lexemAnalyzator;
        static IAscendingAnalyzer analyzer;
        static List<Lab7WorkItem> _Poliz;
        static List<PolizLabel> _PolizLabels;
        static PolizExecutor polizExecutor;

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public JsonResult RelationTable(string grammarText)
        {
            analyzer = new AscendingAnalyzer(grammarText);
            analyzer.SetRelations();
            _Grammar = analyzer.GetGrammar();

            var result = new
            {
                matrix = analyzer.GetRelationMatrix(),
                labels = analyzer.GetAllTerm()
            };

            return Json(result);
        }

        public JsonResult Analyzer(string sourceText)
        {
            lexemAnalyzator = new LexemAnalyzator(dbContext.Lexem.ToList(), sourceText);
            string lexemAnalyzeResult;
            lexemAnalyzator.DoAnalyze(out lexemAnalyzeResult);
            OutTables tables = lexemAnalyzator.GetOutTables();
            if (dbContext.OutLexem.Count() > 0)
            {
                dbContext.OutLexem.RemoveRange(dbContext.OutLexem);
                dbContext.SaveChanges();
            }
            if (dbContext.Identify.Count() > 0)
            {
                dbContext.Identify.RemoveRange(dbContext.Identify);
                dbContext.SaveChanges();
            }
            if (dbContext.Const.Count() > 0)
            {
                dbContext.Const.RemoveRange(dbContext.Const);
                dbContext.SaveChanges();
            }

            dbContext.OutLexem.AddRange(tables._OutLexem);
            dbContext.SaveChanges();
            dbContext.Identify.AddRange(tables._IdentifyTable);
            dbContext.SaveChanges();
            dbContext.Const.AddRange(tables._ConstTable);
            dbContext.SaveChanges();
            var result = new
            {
                lexemAnalyzeResult,
                tables
            };
            return Json(result);
        }

        public JsonResult SyntacticAnalyzer()
        {
            List<OutLexem> lexems = dbContext.OutLexem.ToList();
            bool isLoop = false;
            for (int i = 1; i < lexems.Count() - 1; i++)
            {
                if (lexems[i].Kod == 34)
                    lexems[i].Token = "IDN";
                else if (lexems[i].Kod == 35)
                    lexems[i].Token = "CST";
                else if (lexems[i].Token == "while")
                    isLoop = true;

                if (isLoop)
                    if (lexems[i].Token == "¶")
                    {
                        isLoop = false;
                        lexems.RemoveAt(i);
                        continue;
                    }
            }

            AscendingParser parser = new AscendingParser(analyzer.GetGrammar(), analyzer.GetAllTerm(), analyzer.GetRelationMatrix(), lexems);
            string parserResult = parser.Parse();

            var result = new
            {
                parserResult,
                parserTable = parser.ResultTable
            };
            return Json(result);
        }

        public JsonResult PolizBilder()
        {
            List<OutLexem> outLexems = dbContext.OutLexem.ToList();
            List<Lab7WorkItem> inputVariables = new List<Lab7WorkItem>();
            for (int i = 0; i < outLexems.Count; i++)
            {
                if (outLexems[i].Kod == 34)
                    inputVariables.Add(new Lab7WorkItem(outLexems[i].Token, "IDN"));
                else if (outLexems[i].Kod == 35)
                    inputVariables.Add(new Lab7WorkItem(outLexems[i].Token, "CST"));
                else if (outLexems[i].Token == "-")
                {
                    if (i - 1 >= 0)
                    {
                        if (outLexems[i - 1].Kod != 34 && outLexems[i - 1].Kod != 35 && outLexems[i - 1].Token != ")")
                            inputVariables.Add(new Lab7WorkItem("@", "Operation"));
                        else
                            inputVariables.Add(new Lab7WorkItem(outLexems[i].Token, "Operation"));
                    }
                }
                else
                    inputVariables.Add(new Lab7WorkItem(outLexems[i].Token, "Operation"));
            }
            GlobalPolizBilder bilder = new GlobalPolizBilder(inputVariables);
            bilder.DoWork();
            _Poliz = bilder.Poliz;
            _PolizLabels = bilder.PolizLabels;
            var result = new
            {
                polizStr = bilder.GetPolizString()
            };
            return Json(result);
        }

        public JsonResult PolizExecute(string startParam = "0")
        {
            if (polizExecutor == null)
                polizExecutor = new PolizExecutor(_Poliz, _PolizLabels, dbContext.Identify.ToList());
            if (startParam == "1")
            {
                polizExecutor = new PolizExecutor(_Poliz, _PolizLabels, dbContext.Identify.ToList());
            }
            try
            {
                while (polizExecutor._Status == "")
                {
                    polizExecutor.GoNextStep();
                }
            }
            catch (NullReferenceException ex)
            {
                polizExecutor._Status = ex.Message;
            }
            catch (Exception ex)
            {
                polizExecutor._Status = ex.Message;
            }


            if (polizExecutor._Status.IndexOf("Output") != -1)
            {
                var result = new
                {
                    Status = polizExecutor._Status,
                    OutParam = polizExecutor._OutParam,
                    IdnName = polizExecutor._Stack.Pop().Token
                };
                return Json(result);

            }
            else
            {
                var result = new
                {
                    Status = polizExecutor._Status
                };
                return Json(result);
            }
        }

        public JsonResult OutputParam()
        {
            polizExecutor._Status = "";
            polizExecutor._Position++;
            return PolizExecute();
        }

        public JsonResult InputParam(string param)
        {
            string idnToken = polizExecutor._Stack.Pop().Token;
            var idn = polizExecutor._IdentifyTable.Find(o => o.Token == idnToken);

            if (idn.Type == "int")
            {
                int res;
                if (Int32.TryParse(param, out res))
                    idn.Value = (object)res;
                else throw new ArgumentException();
            }
            else if (idn.Type == "double")
            {
                string temp = "";
                if (param != null && param != "")
                    for (int i = 0; i < param.Length; i++)
                        if (param[i] == '.')
                            temp += ',';
                        else
                            temp += param[i];
                param = temp;

                double res;
                if (Double.TryParse(param, out res))
                    idn.Value = (object)res;
                else
                    throw new ArgumentException();
            }
            else
                throw new ArgumentException();
            polizExecutor._Status = "";
            polizExecutor._Position++;

            return PolizExecute();
        }

    }
}
    