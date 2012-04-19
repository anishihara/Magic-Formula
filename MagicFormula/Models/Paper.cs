using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MagicFormula.Models
{
    public class Stock
    {
        public string Name { get; set; }
        public double PL { get; set; }
        public double ROE { get; set; }
        public int PLPosition { get; set; }
        public int ROEPosition { get; set; }
        public int Position { get;set; }
    }
}