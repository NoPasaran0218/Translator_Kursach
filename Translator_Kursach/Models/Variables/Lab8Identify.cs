using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Translator.Models.Variables
{
    public class Lab8Identify
    {
        public string Token { get; set; }
        public string Type { get; set; }
        public object Value { get; set; }
        public Lab8Identify()
        {

        }

        public Lab8Identify(string token, string type, object value)
        {
            Token = token;
            Type = type;
            Value = value;
        }
    }
}