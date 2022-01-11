using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlankApp.Service
{
    public interface IHistoryService
    {
        
        IList<string> List(int inxex);
        void Refresh();
    }
}
