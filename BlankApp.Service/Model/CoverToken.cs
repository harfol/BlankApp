using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;  
using System.Threading.Tasks;

namespace BlankApp.Service.Model
{
    public class CoverToken
    {
        public string ProjectNumber { get; set; }
        public string Number { get; set; }
        public string[] Titles { get; set; }
        public override string ToString()
        {
            return $"[ZY·ZD·{ProjectNumber}·Y-{Number}]\n" +
                $"[案卷题名:{Titles[0]} ]\n" +
                $"[ {Titles[1]} ]\n" +
                $"[ {Titles[2]} ]\n" +
                $"[ {Titles[3]} ]\n";
        }
    }
}
