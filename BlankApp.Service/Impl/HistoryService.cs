using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlankApp.Service.Impl
{
    public class HistoryService : IHistoryService, IDisposable
    {
        private readonly IConfigurationService _cs;
        private IList<string> _list1 = new List<string>();
        private IList<string> _list2 = new List<string>();
        private IList<string> _list3 = new List<string>();
        private readonly string _historyFile1;
        private readonly string _historyFile2;
        private readonly string _historyFile3;
        public HistoryService(IConfigurationService configurationService)
        {
            this._cs = configurationService;
            _historyFile1 = _cs.AppSettings["HistoryFileName1"];
            _historyFile2 = _cs.AppSettings["HistoryFileName2"];
            _historyFile3 = _cs.AppSettings["HistoryFileName3"];
            
            Refresh();
        }

        public void Dispose()
        {
            string[] his1 = _list1 .Distinct().ToArray();
            string[] his2 = _list2.Distinct().ToArray();
            string[] his3 = _list3.Distinct().ToArray();
            File.WriteAllLines(_historyFile1, his1);
            File.WriteAllLines(_historyFile2, his2);
            File.WriteAllLines(_historyFile3, his3);
        }

        public IList<string> List(int index)
        {
            switch (index)
            {
                case 1: return this._list1;
                case 2: return this._list2;
                case 3: return this._list3;
                default:return null;
            }
        }

        public void Refresh()
        {
            string[] lines1 = File.ReadAllLines(_historyFile1);
            string[] lines2 = File.ReadAllLines(_historyFile2);
            string[] lines3 = File.ReadAllLines(_historyFile3);
            _list1.Clear();
            _list2.Clear();
            _list3.Clear();
            foreach (string s in lines1)
            {
                _list1.Add(s);
            }
            foreach (string s in lines2)
            {
                _list2.Add(s);
            }
            foreach (string s in lines3)
            {
                _list3.Add(s);
            }
        }
    }
}
