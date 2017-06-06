using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Translator.Models.Variables
{
    public class Lab7WorkItem
    {
        public string Token;
        public string Type;
        //public object Value;

        public Lab7WorkItem(string token,string type)
        {
            Type = type;
            Token = token;
            //Value = null;
        }
    }
}