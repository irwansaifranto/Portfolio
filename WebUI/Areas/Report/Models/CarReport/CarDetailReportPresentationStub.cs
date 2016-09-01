using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Business.Entities;
using Common.Enums;

namespace WebUI.Areas.Report.Models
{
    public class CarDetailReportPresentationStub
    {
        public DateTime Date { get; set; }
        public double Income { get; set; }
        public double Expense { get; set; }
        public string Notes { get; set; }
        public Guid? IdExpense { get; set; }

        public CarDetailReportPresentationStub() { }

        public CarDetailReportPresentationStub(expense_item dbItem)
        {
            Date = dbItem.expense.date;
            Income = dbItem.value;
            Expense = 0;
            IdExpense = dbItem.id_expense;
            Notes = dbItem.expense.rent.code;
        }

        public CarDetailReportPresentationStub(car_expense dbItem)
        {
            CarExpenseType type = (CarExpenseType)Enum.Parse(typeof(CarExpenseType), dbItem.expense_type);
            EnumHelper eh = new EnumHelper();

            Date = dbItem.expense_date;
            Income = 0;
            Expense = dbItem.expense_value;
            Notes = eh.GetEnumDescription(type);
        }

        public List<CarDetailReportPresentationStub> MapList(List<expense_item> expenseItems, List<car_expense> carExpenses)
        {
            //kamus
            List<CarDetailReportPresentationStub> carReportList = new List<CarDetailReportPresentationStub>();
            CarDetailReportPresentationStub carReport;

            //algoritma
            foreach (expense_item expenseItem in expenseItems)
            {
                carReport = new CarDetailReportPresentationStub(expenseItem);
                carReportList.Add(carReport);
            }

            foreach (car_expense carExpense in carExpenses)
            {
                carReport = new CarDetailReportPresentationStub(carExpense);
                carReportList.Add(carReport);
            }

            return carReportList;
        }

    }
}


