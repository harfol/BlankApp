using BlankApp.Service.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlankApp.Service
{
    public interface IReportService
    {
        void SaveToXLSX(string savePath, ReportMsg[] reportMsgs,string title);
    }
}
