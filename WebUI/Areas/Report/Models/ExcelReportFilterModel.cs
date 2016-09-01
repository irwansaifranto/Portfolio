using Business.Entities;
using Common.Enums;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Web;
using WebUI.Models.Booking;

namespace WebUI.Areas.Report.Models
{
    public class ExcelReportFilterModel
    {
        public ExcelReportType ReportType { get; set; }
        public System.Guid Id { get; set; }
        public DateTimeOffset BookDate { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string CarModelName { get; set; }
        public string LicensePlate { get; set; }
        public DateTimeOffset StartRent { get; set; }
        public string DriverName { get; set; }
        public int Total { get; set; }
        public int? Discount { get; set; }
        public int? Price { get; set; }
        public string Status { get; set; }
        public string StatusEnum { get; set; }
        public string Employee { get; set; }

        public ExcelReportFilterModel()
        {
        }

        public List<ExcelReportFilterModel> MapList(List<rent> dbItem)
        {
            ExcelReportFilterModel data;
            List<ExcelReportFilterModel> retList = new List<ExcelReportFilterModel>();
            foreach (rent single in dbItem)
            {
                data = new ExcelReportFilterModel();
                data.Id = single.id;
                data.BookDate = single.created_time;
                data.Code = single.code;
                data.Name = single.customer.name;
                data.CarModelName = single.car_model.name;
                data.LicensePlate = single.car != null ? single.car.license_plate : "-";
                data.StartRent = single.start_rent;
                data.DriverName = single.driver != null ? single.driver.name : "-";
                data.Total = single.package_price;
                data.Discount = single.discount;
                data.Discount = single.discount.HasValue ? single.discount.Value : 0;
                data.Price = single.price;
                if (single.status != null)
                {
                    RentStatus rs;
                    rs = (RentStatus)Enum.Parse(typeof(RentStatus), single.status);
                    StatusEnum = rs.ToString();
                    Status = new EnumHelper().GetEnumDescription(rs);
                }
                retList.Add(data);

            }
            return retList;
        }

        public byte[] GenerateExcelReport(List<ExcelReportFilterModel> dataModel)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = new System.Globalization.CultureInfo("en-US");

            //kamus
            XSSFWorkbook workbook = new XSSFWorkbook();
            XSSFSheet sheet; XSSFRow row; XSSFCell cell;
            XSSFCellStyle style;
            XSSFFont font;

            int colIndex; int rowIndex; int lastRow;
            MemoryStream ms = new MemoryStream();
            List<ExcelReportFilterModel> dataReport = dataModel;

            //algoritma
            sheet = (XSSFSheet)workbook.CreateSheet("Report Booking");

            #region header_data
            //row title
            style = (XSSFCellStyle)workbook.CreateCellStyle();
            font = (XSSFFont)workbook.CreateFont();

            style.WrapText = true;
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Top;
            font.Boldweight = (short)FontBoldWeight.Bold;
            style.SetFont(font);

            row = (XSSFRow)sheet.CreateRow((short)0);
            cell = (XSSFCell)row.CreateCell(0);
            cell.SetCellValue("REPORT BOOKING");
            cell.CellStyle = style;

            //create row (header)
            rowIndex = 3;
            row = (XSSFRow)sheet.CreateRow((short)rowIndex);

            //header style
            style = (XSSFCellStyle)workbook.CreateCellStyle();
            font = (XSSFFont)workbook.CreateFont();

            style.WrapText = true;
            style.Alignment = HorizontalAlignment.Center;
            style.VerticalAlignment = VerticalAlignment.Top;
            font.Boldweight = (short)FontBoldWeight.Bold;
            style.SetFont(font);

            style.BorderBottom = BorderStyle.Medium;
            style.BorderTop = BorderStyle.Medium;
            style.BorderRight = BorderStyle.Medium;
            style.BorderLeft = BorderStyle.Medium;

            ///header data first row
            colIndex = 0;
            foreach (string title in new List<string> { "No", "Tanggal Booking", "Kode Booking", "Tamu", "Mobil", "Plat Nomor", "Tanggal Berangkat", "Supir", "Total", "Diskon", "Harga Akhir", "Status" })
            {
                cell = (XSSFCell)row.CreateCell(colIndex);
                cell.SetCellValue(title);
                cell.CellStyle = style;
                ++colIndex;
            }
            #endregion

            #region body
            rowIndex += 1;
            int no = 1;
            foreach (ExcelReportFilterModel data in dataReport)
            {
                row = (XSSFRow)sheet.CreateRow((short)rowIndex);
                style = (XSSFCellStyle)workbook.CreateCellStyle();

                style.Alignment = HorizontalAlignment.Left;
                style.BorderBottom = BorderStyle.Thin;
                style.BorderTop = BorderStyle.Thin;
                style.BorderRight = BorderStyle.Thin;
                style.BorderLeft = BorderStyle.Thin;
                style.VerticalAlignment = VerticalAlignment.Top;
                style.SetFont(font);

                cell = (XSSFCell)row.CreateCell(0);
                cell.SetCellValue(no);
                cell.CellStyle = style;

                cell = (XSSFCell)row.CreateCell(1);
                cell.SetCellValue(data.BookDate.ToString("yyyy/MM/dd"));
                cell.CellStyle = style;

                cell = (XSSFCell)row.CreateCell(2);
                cell.SetCellValue(data.Code);
                cell.CellStyle = style;

                cell = (XSSFCell)row.CreateCell(3);
                cell.SetCellValue(data.Name);
                cell.CellStyle = style;

                cell = (XSSFCell)row.CreateCell(4);
                cell.SetCellValue(data.CarModelName);
                cell.CellStyle = style;

                cell = (XSSFCell)row.CreateCell(5);
                cell.SetCellValue(data.LicensePlate);
                cell.CellStyle = style;

                cell = (XSSFCell)row.CreateCell(6);
                cell.SetCellValue(data.StartRent.ToString("yyyy/MM/dd"));
                cell.CellStyle = style;

                cell = (XSSFCell)row.CreateCell(7);
                cell.SetCellValue(data.DriverName);
                cell.CellStyle = style;

                cell = (XSSFCell)row.CreateCell(8);
                cell.SetCellValue(data.Total.ToString("N0"));
                cell.CellStyle = style;

                cell = (XSSFCell)row.CreateCell(9);
                cell.SetCellValue(((int)data.Discount).ToString("N0"));
                cell.CellStyle = style;

                cell = (XSSFCell)row.CreateCell(10);
                cell.SetCellValue(((int)data.Price).ToString("N0"));
                cell.CellStyle = style;

                cell = (XSSFCell)row.CreateCell(11);
                cell.SetCellValue(data.Status);
                cell.CellStyle = style;

                ++rowIndex;
                ++no;
            }
            #endregion

            sheet.AddMergedRegion(new CellRangeAddress(0, 2, 0, 11));

            ms = new MemoryStream();
            workbook.Write(ms);

            return ms.ToArray();

        }

    }
}