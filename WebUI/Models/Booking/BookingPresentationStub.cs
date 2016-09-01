using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Business.Entities;
using Common.Enums;
using System.ComponentModel;
using System.Web.Script.Serialization;

namespace WebUI.Models.Booking
{
    public class BookingPresentationStub
    {
        public Guid Id { get; set; }
        [DisplayName("Kode Booking")]
        public string Code { get; set; }
        [DisplayName("Nomor Telepon Tamu")]
        public string PhoneNumber { get; set; }
        [DisplayName("Nomor Telepon Tamu 2")]
        public string PhoneNumber2 { get; set; }
        [DisplayName("Nama Tamu")]
        public string Name { get; set; }
        [DisplayName("Alamat Penjemputan")]
        public string PickupLocation { get; set; }
        [DisplayName("Supir")]
        public string DriverName { get; set; }
        [DisplayName("Mobil")]
        public string CarModelName { get; set; }
        [DisplayName("Plat")]
        public string LicensePlate { get; set; }
        [DisplayName("Mulai Sewa")]
        public DateTimeOffset StartRent { get; set; }
        [DisplayName("Selesai Sewa")]
        public DateTimeOffset FinishRent { get; set; }
        [DisplayName("Harga")]
        public int Price { get; set; }

        public string Contact { get; set;}
        public string Logo { get; set; }

        public string Terms { get; set; }
        
        [DisplayName("Catatan")]
        public string Notes { get; set; }
        [DisplayName("Paket Harga")]
        public string PackagePrice { get; set; }
        [DisplayName("Status")]
        public string Status { get; set; }
        [DisplayName("Keterangan Pembatalan")]
        public string CancelNotes { get; set; }
        [DisplayName("Bookers Id")]
        public string CreatedBy { get; set; }
        [DisplayName("Tanggal Input")]
        public DateTimeOffset CreatedTime { get; set; }
        public string StatusEnum { get; set; }
        public bool Summary { get; set; }
        public string UpdateBy { get; set; }
        public DateTime UpdatedTimeUtc { get; set; }
        public Guid? IdCarpackage { get; set; }
        public Guid IdRentPackage { get; set; }
        public Guid IdRent { get; set; }
        public Guid IdCarPackage { get; set; }
        public int Quantity { get; set; }
        public int PriceEach { get; set; }
        public List<rent_package> ListRentPackage { get; set; }

        //public DateTime StartUtc
        //{
        //    get
        //    {
        //        return new DateTime(StartRent.Year, StartRent.Month, StartRent.Day, StartRent.Hour, StartRent.Minute, StartRent.Second);
        //    }
        //}
        //public DateTime FinishUtc
        //{
        //    get
        //    {
        //        return new DateTime(FinishRent.Year, FinishRent.Month, FinishRent.Day, FinishRent.Hour, FinishRent.Minute, FinishRent.Second);
        //    }
        //}
		
		public BookingPresentationStub() { }

        public BookingPresentationStub(rent dbItem)
        {
            //kamus
            string pn = dbItem.customer.phone_number.Replace('_',' ');
            RentStatus rs;
            string[] arrPn;

            //algoritma
            if (pn != null)
            {
                arrPn = pn.Split(';');
                pn = string.Join("<br>", arrPn);
            }

            Id = dbItem.id;
            Code = dbItem.code;
            PhoneNumber = pn;
            IdCarpackage = dbItem.id_car_package;

            Name = dbItem.customer.name;
            PickupLocation = dbItem.pickup_location;
            Logo = dbItem.owner.logo;
            Contact = dbItem.owner.contact;
            Terms = dbItem.owner.terms;
            if (dbItem.id_driver.HasValue)
                DriverName = dbItem.driver.name;
            CarModelName = dbItem.car_model.name;
            if (dbItem.id_car.HasValue)
                LicensePlate = dbItem.car.license_plate;
            StartRent = dbItem.start_rent;
            FinishRent = dbItem.finish_rent;
            Price = dbItem.price;
            Notes = dbItem.notes;

            if (dbItem.status != null)
            {
                rs = (RentStatus)Enum.Parse(typeof(RentStatus), dbItem.status);
                StatusEnum = rs.ToString();
                Status = new EnumHelper().GetEnumDescription(rs);
            }

            CancelNotes = dbItem.cancel_notes;
            CreatedBy = dbItem.created_by;
            CreatedTime = dbItem.created_time;
            UpdateBy = dbItem.updated_by;
            
            if (dbItem.updated_time.HasValue)
                UpdatedTimeUtc = dbItem.updated_time.Value.UtcDateTime;

        }

        public BookingPresentationStub(rent dbItem, List<rent_package> listRentPackage)
        {
            //kamus
            string pn = dbItem.customer.phone_number.Replace('_', ' ');
            RentStatus rs;
            string[] arrPn;

            //algoritma
            if (pn != null)
            {
                arrPn = pn.Split(';');
                pn = string.Join("<br>", arrPn);
            }

            Id = dbItem.id;
            Code = dbItem.code;
            PhoneNumber = pn;
            //IdCarpackage = dbItem.id_car_package;

            Name = dbItem.customer.name;
            PickupLocation = dbItem.pickup_location;
            Logo = dbItem.owner.logo;
            Contact = dbItem.owner.contact;
            Terms = dbItem.owner.terms;
            if (dbItem.id_driver.HasValue)
                DriverName = dbItem.driver.name;
            CarModelName = dbItem.car_model.name;
            if (dbItem.id_car.HasValue)
                LicensePlate = dbItem.car.license_plate;
            StartRent = dbItem.start_rent;
            FinishRent = dbItem.finish_rent;
            Price = dbItem.price;
            Notes = dbItem.notes;

            if (dbItem.status != null)
            {
                rs = (RentStatus)Enum.Parse(typeof(RentStatus), dbItem.status);
                StatusEnum = rs.ToString();
                Status = new EnumHelper().GetEnumDescription(rs);
            }

            CancelNotes = dbItem.cancel_notes;
            CreatedBy = dbItem.created_by;
            CreatedTime = dbItem.created_time;
            UpdateBy = dbItem.updated_by;

            if (dbItem.updated_time.HasValue)
                UpdatedTimeUtc = dbItem.updated_time.Value.UtcDateTime;

            ListRentPackage = listRentPackage;

        }

		public List<BookingPresentationStub> MapList(List<rent> dbItems)
        {
            List<BookingPresentationStub> retList = new List<BookingPresentationStub>();

            foreach (rent dbItem in dbItems)
                retList.Add(new BookingPresentationStub(dbItem));

            return retList;
        }

        
	}
}

