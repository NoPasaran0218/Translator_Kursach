using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Translator.Models
{
    public class Const
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public Const()
        {

        }
        public Const(int id, string token)
        {
            Id = id;
            Token = token;
        }
    }
}