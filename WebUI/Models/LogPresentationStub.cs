using Business.Entities;
using Common.Enums;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;

namespace WebUI.Models
{
    public class LogPresentationStub
    {
        public long Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Ip { get; set; }
        public string User { get; set; }
        public string Action { get; set; }
        public string Data { get; set; }
        public string Application { get; set; }

        public LogPresentationStub() { }

        public LogPresentationStub(log dbItem)
        {
            Id = dbItem.id;
            Timestamp = dbItem.timestamp;
            Ip = dbItem.ip;
            User = dbItem.user;
            Action = dbItem.action;
            Data = System.Net.WebUtility.UrlDecode(dbItem.data);
            Application = dbItem.application;
        }

        public List<LogPresentationStub> MapList(List<log> dbItems)
        {
            List<LogPresentationStub> retList = new List<LogPresentationStub>();

            foreach (log c in dbItems)
                retList.Add(new LogPresentationStub(c));

            return retList;
        }

        public MemoryStream GenerateExcel(List<LogPresentationStub> items)
        {
            //kamus lokal
            int rowIndex = 0, colIndex;
            XSSFCellStyle styleHeader, styleDate; XSSFFont font;
            XSSFRow row; XSSFCell cell;

            //algoritma
            Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");
            XSSFWorkbook workbook = new XSSFWorkbook();
            XSSFSheet sheet = (XSSFSheet)workbook.CreateSheet("activity log");

            //create row (header)
            row = (XSSFRow)sheet.CreateRow((short)rowIndex++);

            //header style
            styleHeader = (XSSFCellStyle)workbook.CreateCellStyle();
            font = (XSSFFont)workbook.CreateFont();
            font.Boldweight = (short)FontBoldWeight.Bold; ;
            styleHeader.SetFont(font);

            //header data
            List<string> colNames = new List<string> { "id", "timestamp", "application", "ip", "user", "action", "data" };
            colIndex = 0;
            foreach (string single in colNames)
            {
                cell = (XSSFCell)row.CreateCell(colIndex++);
                cell.SetCellValue(single);
                cell.CellStyle = styleHeader;
            }

            //body
            styleDate = (XSSFCellStyle)workbook.CreateCellStyle();
            styleDate.DataFormat = workbook.CreateDataFormat().GetFormat("yyyy-mm-dd HH:mm");
            foreach (LogPresentationStub single in items)
            {
                row = (XSSFRow)sheet.CreateRow((short)rowIndex++);
                colIndex = 0;

                cell = (XSSFCell)row.CreateCell(colIndex++);
                cell.SetCellValue(single.Id);

                cell = (XSSFCell)row.CreateCell(colIndex++);
                cell.SetCellValue(single.Timestamp);
                cell.CellStyle = styleDate;

                cell = (XSSFCell)row.CreateCell(colIndex++);
                cell.SetCellValue(single.Application);

                cell = (XSSFCell)row.CreateCell(colIndex++);
                cell.SetCellValue(single.Ip);

                cell = (XSSFCell)row.CreateCell(colIndex++);
                cell.SetCellValue(single.User);

                cell = (XSSFCell)row.CreateCell(colIndex++);
                cell.SetCellValue(single.Action);

                cell = (XSSFCell)row.CreateCell(colIndex++);
                cell.SetCellValue(single.Data);
            }

            //write to file
            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);

            return ms;
        }
    }
}