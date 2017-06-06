using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Translator.Models
{
    public class OutLexem
    {
        public int Id { get; set; }
        public int Row { get; set; }
        public string Token { get; set; }
        public int Kod { get; set; }
        public Nullable<int> KodIdn_Konst { get; set; }
        public OutLexem()
        {

        }
        public OutLexem(int id, int row, string token, int kod, int? kodidnKonst)
        {
            Id = id;
            Row = row;
            Token = token;
            Kod = kod;
            KodIdn_Konst = kodidnKonst;
        }
    }
}