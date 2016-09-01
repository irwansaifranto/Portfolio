using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business.Entities;
using Common.Enums;
using WebUI.Infrastructure.Validation;
using WebUI.Infrastructure.Concrete;
using WebUI.Models.CarPackage;
using System.Web.Script.Serialization;

namespace WebUI.Models.Booking
{
    public class BookingFormStub
    {
        // Example model value from scaffolder script: 0
        [DisplayName("Id")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public System.Guid Id { get; set; }

        [DisplayName("Kode Booking")]
        public string Code { get; set; }

        [DisplayName("Nomor Telepon Tamu")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public string PhoneNumber { get; set; }

        [DisplayName("Nomor Telepon Tamu 2")]
        public string PhoneNumber2 { get; set; }

        public Guid? IdCustomer { get; set; }

        public CustomerTitle? CustomerTitle { get; set; }

        [DisplayName("Nama Tamu")]
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public string Name { get; set; }

        [DisplayName("Alamat Tamu")]
        public string Address { get; set; }

        [DisplayName("Alamat Penjemputan")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public string PickupLocation { get; set; }

        [DisplayName("Supir")]
        public Guid? IdDriver { get; set; }

        [DisplayName("Mobil")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public Guid? IdCarModel { get; set; }

        [DisplayName("Plat")]
        public Guid? IdCar { get; set; }

        [DisplayName("Mulai Sewa")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public DateTimeOffset StartRent { get; set; }

        [DisplayName("Selesai Sewa")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        [DateOffsetGreater("StartRent")]
        public DateTimeOffset FinishRent { get; set; }

        [DisplayName("Paket")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public Guid IdCarPackage { get; set; }

        //[DisplayName("Paket")]
        //public string Paket { get; set; }

        [DisplayName("Diskon")]
        public int? Discount { get; set; }

        [DisplayName("Total")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public int? PackagePrice { get; set; }

        [DisplayName("Harga Akhir")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public int? Price { get; set; }

        [DisplayName("Catatan")]
        [DataType(DataType.MultilineText)]
        [AllowHtml]
        public string Notes { get; set; }

        [DisplayName("Status")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        public RentStatus Status { get; set; }


        [DisplayName("Keterangan Pembatalan")]
        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(Resources.MyGlobalErrors))]
        [DataType(DataType.MultilineText)]
        [AllowHtml]
        public string CancelNotes { get; set; }
        [DisplayName("Daftar Paket Harga")]
        public string ListRentPackageText { get; set; }
        public List<RentPackageFormStub> ListRentPackageItem { get; set; }
        
        public BookingFormStub()
        {
            DateTime now = DateTime.Now;
            StartRent = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);
            StartRent = StartRent.AddHours(1);

            FinishRent = StartRent.AddDays(1);

            Code = "- dibuat otomatis oleh sistem -";
        }


        public BookingFormStub(rent dbItem, List<car_package> listPackage)
        {
            EnumHelper eh = new EnumHelper();
            car_package package = new car_package();
            Id = dbItem.id;
            Code = dbItem.code;
            PickupLocation = dbItem.pickup_location;
            IdDriver = dbItem.id_driver;
            IdCarModel = dbItem.id_car_model;
            IdCar = dbItem.id_car;

            StartRent = dbItem.start_rent;
            FinishRent = dbItem.finish_rent;
            PackagePrice = dbItem.package_price;
            Discount = dbItem.discount;
            if (dbItem.id_car_package.HasValue)
                IdCarPackage = dbItem.id_car_package.Value;
            Price = dbItem.price;
            Notes = dbItem.notes;
            Status = (RentStatus)Enum.Parse(typeof(RentStatus), dbItem.status);
            CancelNotes = dbItem.cancel_notes;

            //customer data            
            if (dbItem.customer.phone_number != null)
            {
                string[] words = dbItem.customer.phone_number.Split(';');
                this.PhoneNumber = words[0];

                if (words.Count() > 1)
                {
                    this.PhoneNumber2 = words[1];
                }
            }
            IdCustomer = dbItem.id_customer;
            Name = dbItem.customer.name;
            Address = dbItem.customer.address;
            if (dbItem.customer.title != null)
            {
                CustomerTitle = (CustomerTitle)Enum.Parse(typeof(CustomerTitle), dbItem.customer.title);
            }

            ListRentPackageItem = new List<RentPackageFormStub>();
            foreach (rent_package single in dbItem.rent_package)
            {
                ListRentPackageItem.Add(new RentPackageFormStub(single));
            }
            ListRentPackageText = new JavaScriptSerializer().Serialize(ListRentPackageItem);

        }

        public rent GetDbObjectOnCreate(string username, Guid idOwner)
        {
            rent dbItem = new rent
            {
                cancel_notes = CancelNotes,
                code = Code,
                created_by = username,
                created_time = DateTimeOffset.Now,
                finish_rent = FinishRent,
                id_car = IdCar,
                id_car_model = IdCarModel.Value,
                id_customer = IdCustomer.Value,
                id_driver = IdDriver,
                id_owner = idOwner,
                notes = Notes,
                pickup_location = PickupLocation,
                price = Price.Value,
                start_rent = StartRent,
                status = Status.ToString(),
                discount = Discount,
                updated_by = username,
                updated_time = DateTimeOffset.Now,
                id_car_package = IdCarPackage,
                package_price = (int)PackagePrice

            };

            return dbItem;
        }

        public void UpdateDbObject(rent dbItem, CustomPrincipal user)
        {
            dbItem.cancel_notes = CancelNotes;
            dbItem.finish_rent = FinishRent;
            dbItem.id_car = IdCar;
            dbItem.id_car_model = IdCarModel.Value;
            dbItem.id_customer = IdCustomer.Value;
            dbItem.id_driver = IdDriver;
            dbItem.notes = Notes;
            dbItem.pickup_location = PickupLocation;
            dbItem.price = Price.Value;
            dbItem.start_rent = StartRent;
            dbItem.status = Status.ToString();
            dbItem.updated_by = user.Identity.Name;
            dbItem.discount = Discount;
            dbItem.updated_time = DateTimeOffset.Now;
            dbItem.id_car_package = IdCarPackage;
            dbItem.package_price = (int)PackagePrice;
        }

        #region customer

        public customer CreateNewCustomer(Guid idOwner)
        {
            string phone2 = "";
            if (PhoneNumber2 != null) { phone2 = PhoneNumber2; }

            customer cust = new customer
            {
                name = Name,
                address = Address,
                phone_number = PhoneNumber.Replace("_", "") + ";" + phone2,
                id_owner = idOwner
            };
            if (CustomerTitle != null)
                cust.title = CustomerTitle.Value.ToString();

            return cust;
        }

        public void UpdateCustomer(customer cust)
        {

            string phone2 = "";

            if (PhoneNumber2 != null) { phone2 = PhoneNumber2; }

            cust.phone_number = PhoneNumber.Replace("_", "") + ";" + phone2;
            cust.address = Address;

            if (CustomerTitle != null)
                cust.title = CustomerTitle.Value.ToString();
        }

        #endregion

        #region options

        public List<SelectListItem> GetStatusOptions()
        {
            EnumHelper eh = new EnumHelper();
            List<RentStatus> list = eh.EnumToList<RentStatus>().ToList();
            List<SelectListItem> options = new List<SelectListItem>();

            foreach (RentStatus row in list)
                options.Add(new SelectListItem { Text = eh.GetEnumDescription(row), Value = row.ToString() });

            return options;
        }

        public List<SelectListItem> GetCustomerTitleOptions()
        {
            EnumHelper eh = new EnumHelper();
            List<CustomerTitle> list = eh.EnumToList<CustomerTitle>().ToList();
            List<SelectListItem> options = new List<SelectListItem>();

            foreach (CustomerTitle row in list)
                options.Add(new SelectListItem { Text = eh.GetEnumDescription(row), Value = row.ToString() });

            return options;
        }

        private List<CarPackagePresentationStub> CarPackageOptions { get; set; }
        public List<CarPackagePresentationStub> GetCarPackageOptions()
        {
            return CarPackageOptions;
        }
        public string GetCarPackageOptionsAsJson()
        {
            return new JavaScriptSerializer().Serialize(CarPackageOptions);
        }
        public void SetCarPackageOptions(List<car_package> dbItems)
        {
            CarPackageOptions = new CarPackagePresentationStub().MapList(dbItems);
        }

        #endregion
    }
}

