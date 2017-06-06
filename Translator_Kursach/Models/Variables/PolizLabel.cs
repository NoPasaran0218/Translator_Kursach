using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Translator.Models.Variables
{
    public class PolizLabel
    {
        public string Label;
        public int Position;
        public PolizLabel(string label, int position)
        {
            Label = label;
            Position = position;
        }
    }
}