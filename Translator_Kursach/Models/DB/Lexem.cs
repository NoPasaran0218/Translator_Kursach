using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Translator.Models
{
    public class Lexem
    {
        public Lexem()
        {
            this.Compatible = new HashSet<Compatible>();
        }

        public Lexem(int id, string token, bool separator)
        {
            Id = id;
            Token = token;
            Separator = separator;
            Compatible = new HashSet<Compatible>();
        }

        public int Id { get; set; }
        public string Token { get; set; }
        public bool Separator { get; set; }

        public virtual ICollection<Compatible> Compatible { get; set; }
    }
}