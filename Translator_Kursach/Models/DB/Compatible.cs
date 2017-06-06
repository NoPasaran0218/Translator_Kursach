using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Translator.Models
{
    public class Compatible
    {
        public int Id { get; set; }
        public string CanJoin { get; set; }
        public int LexemId { get; set; }

        public Compatible() { }

        public Compatible(int id, string canJoin, int lexemId)
        {
            Id = id;
            CanJoin = canJoin;
            LexemId = lexemId;
        }
        public virtual Lexem Lexem { get; set; }
    }
}