using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Translator.Models.Variables
{
    public class OutTables
    {
        public List<OutLexem> _OutLexem { get; set; }
        public List<Identify> _IdentifyTable { get; set; }
        public List<Const> _ConstTable { get; set; }

        public OutTables(List<OutLexem> outLexem, List<Identify> identifyTable, List<Const> constTable)
        {
            _OutLexem = outLexem;
            _IdentifyTable = identifyTable;
            _ConstTable = constTable;
        }
    }
}