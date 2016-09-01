using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Business.Entities;
using Common.Enums;

namespace WebUI.Areas.Report.Models
{
    public class CarReportPresentationStub
    {
        // Example model value from scaffolder script: 0
        public System.Guid IdCarModel { get; set; }
        public System.Guid? IdCar { get; set; }
        public string LicensePlate { get; set; }
        public string CarModelName { get; set; }
        public string CarBrandName { get; set; }
        public double Income { get; set; }
        public double Expense { get; set; }
        public double Margin { get; set; }
        public int CarUsed { get; set; }

        public CarReportPresentationStub() { }

        public CarReportPresentationStub(car dbItem)
        {
            this.IdCarModel = dbItem.id_car_model;
            this.IdCar = dbItem.id;
            this.LicensePlate = dbItem.license_plate;
            this.CarModelName = dbItem.car_model != null ? dbItem.car_model.name : "";
            this.CarBrandName = dbItem.car_model.car_brand.name;
        }

        public List<CarReportPresentationStub> MapList(List<car> cars, List<rent> rents, List<expense_item> expenseItems, List<car_expense> carExpenses)
        {
            List<CarReportPresentationStub> carReportList = new List<CarReportPresentationStub>();
            CarReportPresentationStub single;

            foreach (car car in cars)
            {
                single = new CarReportPresentationStub(car);

                single.CarUsed = rents.Where(n => n.id_car != null && n.id_car.Value == car.id).Count();
                single.Income = (double)expenseItems.Where(m => m.expense.rent.id_car != null && m.expense.rent.id_car.Value == car.id).Sum(n => n.value);
                single.Expense = carExpenses.Where(n => n.id_car == car.id).Sum(n => n.expense_value);
                single.Margin = single.Income - single.Expense;

                carReportList.Add(single);
            }

            return carReportList;


        }

    }
}


