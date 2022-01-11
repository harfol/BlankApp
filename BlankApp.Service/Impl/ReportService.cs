using BlankApp.Service.Model;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlankApp.Service.Impl
{
    public class ReportService : IReportService
    {
        public void SaveToXLSX(string savePath, ReportMsg[] reportMsgs, string title)
        {


            XSSFWorkbook xssfworkbook = new XSSFWorkbook();
            ISheet sheet = xssfworkbook.CreateSheet("Sheet");
            string date = DateTime.Now.ToString("g").Replace(' ', '\n');

            ICellStyle cellStyleHeaderLeft;
            ICellStyle cellStyleHeaderCenter;
            ICellStyle cellStyleHeaderRight;
            ICellStyle cellStyleTextLeft;
            ICellStyle cellStyleTextCenter;
            ICellStyle cellStyleTextRight;
            ICellStyle cellDateStyle;
            ICellStyle cellTitleStyle;
            ICellStyle cellPageStyle;

            #region 单元格格式
            cellStyleHeaderLeft = xssfworkbook.CreateCellStyle();
            cellStyleHeaderLeft.VerticalAlignment = VerticalAlignment.Center;
            cellStyleHeaderLeft.Alignment = HorizontalAlignment.Center;
            cellStyleHeaderLeft.BorderLeft = BorderStyle.Thick;
            cellStyleHeaderLeft.BorderTop = BorderStyle.Thick;
            cellStyleHeaderLeft.BorderRight = BorderStyle.Thin;
            cellStyleHeaderLeft.BorderBottom = BorderStyle.Thick;

            cellStyleHeaderCenter = xssfworkbook.CreateCellStyle();
            cellStyleHeaderCenter.VerticalAlignment = VerticalAlignment.Center;
            cellStyleHeaderCenter.Alignment = HorizontalAlignment.Center;
            cellStyleHeaderCenter.BorderLeft = BorderStyle.Thin;
            cellStyleHeaderCenter.BorderTop = BorderStyle.Thick;
            cellStyleHeaderCenter.BorderRight = BorderStyle.Thin;
            cellStyleHeaderCenter.BorderBottom = BorderStyle.Thick;

            cellStyleHeaderRight = xssfworkbook.CreateCellStyle();
            cellStyleHeaderRight.VerticalAlignment = VerticalAlignment.Center;
            cellStyleHeaderRight.Alignment = HorizontalAlignment.Center;
            cellStyleHeaderRight.BorderLeft = BorderStyle.Thin;
            cellStyleHeaderRight.BorderTop = BorderStyle.Thick;
            cellStyleHeaderRight.BorderRight = BorderStyle.Thick;
            cellStyleHeaderRight.BorderBottom = BorderStyle.Thick;

            cellStyleTextLeft = xssfworkbook.CreateCellStyle();
            cellStyleTextLeft.VerticalAlignment = VerticalAlignment.Center;
            cellStyleTextLeft.Alignment = HorizontalAlignment.Center;
            cellStyleTextLeft.BorderLeft = BorderStyle.Thick;
            cellStyleTextLeft.BorderTop = BorderStyle.None;
            cellStyleTextLeft.BorderRight = BorderStyle.Thin;
            cellStyleTextLeft.BorderBottom = BorderStyle.Thin;

            cellStyleTextCenter = xssfworkbook.CreateCellStyle();
            cellStyleTextCenter.VerticalAlignment = VerticalAlignment.Center;
            cellStyleTextCenter.Alignment = HorizontalAlignment.Center;
            cellStyleTextCenter.BorderLeft = BorderStyle.Thin;
            cellStyleTextCenter.BorderTop = BorderStyle.None;
            cellStyleTextCenter.BorderRight = BorderStyle.Thin;
            cellStyleTextCenter.BorderBottom = BorderStyle.Thin;

            cellStyleTextRight = xssfworkbook.CreateCellStyle();
            cellStyleTextRight.VerticalAlignment = VerticalAlignment.Center;
            cellStyleTextRight.Alignment = HorizontalAlignment.Center;
            cellStyleTextRight.BorderLeft = BorderStyle.Thin;
            cellStyleTextRight.BorderTop = BorderStyle.None;
            cellStyleTextRight.BorderRight = BorderStyle.Thick;
            cellStyleTextRight.BorderBottom = BorderStyle.Thin;

            cellDateStyle = xssfworkbook.CreateCellStyle();
            cellDateStyle.VerticalAlignment = VerticalAlignment.Center;
            cellDateStyle.Alignment = HorizontalAlignment.Center;
            cellDateStyle.Rotation = (short)(-90);
            cellDateStyle.WrapText = true;

            cellTitleStyle = xssfworkbook.CreateCellStyle();
            cellTitleStyle.VerticalAlignment = VerticalAlignment.Center;
            cellTitleStyle.Alignment = HorizontalAlignment.Center;
            cellTitleStyle.Rotation = (short)255;

            cellPageStyle = xssfworkbook.CreateCellStyle();
            cellPageStyle.VerticalAlignment = VerticalAlignment.Center;
            cellPageStyle.Alignment = HorizontalAlignment.Center;
            #endregion

            for (int p = 0; p < reportMsgs.Length; p+=170)
            {
                int page = p / 170;

                // header
                IRow row = sheet.CreateRow(page*35);
                for (int i = 0; i < 5; i++)
                {
                    ICell cell1 = row.CreateCell(i * 3);
                    cell1.CellStyle = cellStyleHeaderLeft;
                    cell1.SetCellValue("序号");
                    ICell cell2 = row.CreateCell(i * 3 + 1);
                    cell2.CellStyle = cellStyleHeaderCenter;
                    cell2.SetCellValue("扫描");
                    ICell cell3 = row.CreateCell(i * 3 + 2);
                    cell3.CellStyle = cellStyleHeaderRight;
                    cell3.SetCellValue("总数");
                }
                // data 
                for (int i = 0; i < 34; i++)
                {
                    IRow r = sheet.CreateRow(page*35 + i + 1);
                    for (int j = 0; j < 5; j++)
                    {
                        int index = p + 34 * j + i;
                        ICell cell1 = r.CreateCell(j * 3);
                        cell1.CellStyle = cellStyleTextLeft;
                        ICell cell2 = r.CreateCell(j * 3 + 1);
                        cell2.CellStyle = cellStyleTextCenter;
                        ICell cell3 = r.CreateCell(j * 3 + 2);
                        cell3.CellStyle = cellStyleTextRight;
                        
                        if (index < reportMsgs.Length)
                        {
                            cell1.SetCellValue(string.Format("{0:D4}", reportMsgs[index].Id));
                            cell2.SetCellValue(reportMsgs[index].Scan);
                            cell3.SetCellValue(reportMsgs[index].Sum);
                        }
                    }
                }
                // 标题，时间空格
                for (int i = 0; i < 35; i++)
                {
                    sheet.GetRow(i + page*35).CreateCell(15);
                }
                // 标题
                ICell cellTitle = sheet.GetRow(page * 35).GetCell(15);
                cellTitle.SetCellValue(title);
                cellTitle.CellStyle = cellTitleStyle;
                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(page * 35, page * 35 + 25, 15, 15)); // 

                // 时间
                ICell cellDate = sheet.GetRow(page * 35 + 26).GetCell(15);
                cellDate.SetCellValue(date);
                cellDate.CellStyle = cellDateStyle;
                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(page * 35 + 26, page * 35 + 33, 15, 15));

                //页码
                ICell cellPage = sheet.GetRow(page * 35 + 34).GetCell(15);
                cellPage.CellStyle = cellPageStyle;
                cellPage.SetCellValue(string.Format("{0}/{1}", page+1, reportMsgs.Length/170 + 1));

            }
           
            using (FileStream fs = new FileStream(savePath, FileMode.Create, FileAccess.Write))
            {
                xssfworkbook.Write(fs);
                fs.Close();
            }
        }
    }
}
