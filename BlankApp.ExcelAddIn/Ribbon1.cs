using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Tools.Ribbon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlankApp.ExcelAddIn
{
    public partial class Ribbon1
    {


        private Form1 _form;
        public Application App { get; private set; } = null;
        private void Ribbon1_Load(object sender, RibbonUIEventArgs e)
        {
            App = Globals.ThisAddIn.Application;
        }

        private void btnCreateSQL_Click(object sender, RibbonControlEventArgs e)
        {
            int sr = App.ActiveCell.Row;   // 选中的最低行
            int sc = App.ActiveCell.Column;    // 选中的最低列            
            int height = App.Selection.Rows.Count;    // 行数
            int width = App.Selection.Columns.Count;    // 列数
            Worksheet sheet = App.ActiveSheet as Worksheet;    // 当前活动表

            // { {index1,[x1,x2,x3...]}，{index2,[x1,x2,x3...]} ..}    大概这种分组方法
            Dictionary<string, List<string>> o = new Dictionary<string, List<string>>();
            for (int i = 0; i < width; i++)
            {
                string title = sheet.Cells[1, sc + i].Text;   // 列头
                if( string.IsNullOrWhiteSpace(title))    // 去空行
                {
                    break;
                }
                else if( title.Contains(',') )    // ,号做标志位
                {
                    string dbname = title.Split(',')[1];
                    List<string> list = new List<string>();
                    for (int j = 0; j < height; j++)
                    {
                        string value = sheet.Cells[sr +j, sc+i].Text;
                        list.Add(value);
                    }
                    o.Add(dbname, list);
                }
            }
            if( _form ==null || _form.IsClose )
            {
                _form = new Form1(o);
                _form.Show();
                _form.IsClose = false;
            }
            else
            {
                _form.SqlModels = o;
                _form.TempModels.Clear();
                _form.ShowSQL();
                _form.Activate();
            }
        }
    }
}
