using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Translator.Models
{
    public class Automatic
    {
        public int Id { get; set; }
        public int A { get; set; }
        public string Token { get; set; }
        public Nullable<int> B { get; set; }
        public Nullable<int> Stack { get; set; }
        public string AdditionInform { get; set; }
        public Automatic()
        {

        }
        public Automatic(int id, int a, string token, int? b, int? stack, string additionInform)
        {
            Id = id;
            A = a;
            Token = token;
            B = b;
            Stack = stack;
            AdditionInform = additionInform;
        }
    }
}