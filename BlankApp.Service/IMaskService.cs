using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlankApp.Service
{   
    public interface IMaskService
    {
        string Simplify(string str);
        string Complex(string str);

        string SimplifiedNumbers(int[] nums);

        int[] ComplexNumbers(string str);
    }
}
