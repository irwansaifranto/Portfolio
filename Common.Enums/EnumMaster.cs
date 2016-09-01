using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Enums
{
    public enum Day
    {
        SUNDAY = 0,
        MONDAY = 1,
        TUESDAY = 2,
        WEDNESDAY = 3,
        THURSDAY = 4,
        FRIDAY = 5,
        SATURDAY = 6,
    }

    public enum CustomerType
    {
        [Description("Personal")]
        PERSONAL,
        [Description("Corporate")]
        CORPORATE
    }

    public enum CustomerTitle
    {
        [Description("Bpk")]
        MR,
        [Description("Ibu")]
        MRS
    }

    public enum DriverType
    {
        [Description("Regular")]
        PERSONAL,
        [Description("Part Time")]
        PARTTIME
    }

    public enum RentStatus
    {
        [Description("Belum Jalan")]
        NEW,
        [Description("Sudah Jalan")]
        GO,
        [Description("Selesai")]
        FINISH,
        [Description("Batal")]
        CANCEL
    }

    public enum InvoiceStatus
    {
        [Description("Sudah Dibayar")]
        PAID,
        [Description("Belum Dibayar")]
        UNPAID,
        [Description("Batal")]
        CANCEL
    }

    public enum ExpenseItemCategory
    {
        [Description("Mobil")]
        VEHICLE,
        [Description("Supir")]
        DRIVER,
        [Description("Bensin")]
        GAS,
        [Description("Tarif Tol")]
        TOLL,
        [Description("Parkir")]
        PARKING,
        [Description("Biaya Lain-Lain")]
        OTHER
    }

    public enum CarTransmission
    {
        [Description("Matic")]
        AT,
        [Description("Manual")]
        MT
    }

    public enum CarFuel
    {
        [Description("Bensin")]
        GASOLINE,
        [Description("Diesel")]
        DIESEL,
        [Description("Lainnya")]
        OTHER
    }

    //public enum Title
    //{
    //    [Description("Bapak")]
    //    MR,
    //    [Description("Ibu")]
    //    Mrs
    //}

    public enum ReportType
    {
        [Description("Bulanan")]
        MONTHLY,
        [Description("Harian")]
        DAILY
    }

    public enum CarExpenseType
    {
        [Description("Asuransi")]
        INSURANCE,
        [Description("Pajak")]
        TAX,
        [Description("STNK")]
        STNK,
        [Description("Maintenance")]
        MAINTENANCE,
        [Description("Lain-lain")]
        OTHER
    }

    public enum ExcelReportType
    {
        [Description("Booking")]
        RENT
    }

    #region api

    public enum ApiRentStatus
    {
        [Description("Baru")]
        NEW,
        [Description("Konfirmasi")]
        CONFIRM
    }

    public enum ApiRentCancellationStatus
    {
        [Description("Batal")]
        CANCEL
    }

    #endregion
}
