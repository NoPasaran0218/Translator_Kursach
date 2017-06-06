using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Translator.Models
{
    public class Identify
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public string Type { get; set; }
        public Identify()
        {

        }
        public Identify(int id, string token, string type)
        {
            Id = id;
            Token = token;
            Type = type;
        }
    }
}