using Business.Entities;
using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.WebPages.Html;

namespace WebUI.Models.ApiRent
{
    public class ApiRentPresentationStub
    {
        public Guid Id { get; set; }
        public Guid IdRent { get; set; }
        public DateTimeOffset BookDate { get; set; }
        public string Partner { get; set; }
        public string Rental { get; set; }
        public string BookingCode { get; set; }
        public DateTimeOffset Departure { get; set; }
        public string Status { get; set; }
        public string CancellationStatus { get; set; }

        #region options

        private List<SelectListItem> StatusOptions { get; set; }
        private List<SelectListItem> CancelOptions { get; set; }

        public void FillStatusOptions()
        {
            StatusOptions = new List<SelectListItem>();
            EnumHelper enumHelper = new EnumHelper();
            foreach (ApiRentStatus item in enumHelper.EnumToList<ApiRentStatus>().ToList())
            {
                StatusOptions.Add(new SelectListItem { Value = item.ToString(), Text = enumHelper.GetEnumDescription(item) });
            }
        }
        public void FillCancelOptions()
        {
            CancelOptions = new List<SelectListItem>();
            EnumHelper enumHelper = new EnumHelper();
            foreach (ApiRentCancellationStatus item in enumHelper.EnumToList<ApiRentCancellationStatus>().ToList())
            {
                CancelOptions.Add(new SelectListItem { Value = item.ToString(), Text = enumHelper.GetEnumDescription(item) });
            }
        }

        public string GetStatusOptionsAsJson()
        {
            return new JavaScriptSerializer().Serialize(StatusOptions);
        }

        public string GetCancelOptionsAsJson()
        {
            return new JavaScriptSerializer().Serialize(CancelOptions);
        }

        #endregion

        public ApiRentPresentationStub () { }

        public ApiRentPresentationStub(api_rent dbItem)
        {
            ApiRentStatus status = (ApiRentStatus)Enum.Parse(typeof(Common.Enums.ApiRentStatus), dbItem.status);
            
            Id = dbItem.id;
            IdRent = dbItem.id_rent;
            BookDate = dbItem.rent.created_time;
            Partner = dbItem.partner;
            Rental = dbItem.rent.owner.name;
            BookingCode = dbItem.rent.code;
            Departure = dbItem.rent.start_rent;
            Status = new EnumHelper().GetEnumDescription(status);
            if (dbItem.cancellation_status != null)
            {
                ApiRentCancellationStatus cancelStatus = (ApiRentCancellationStatus)Enum.Parse(typeof(Common.Enums.ApiRentCancellationStatus), dbItem.cancellation_status);
                CancellationStatus = new EnumHelper().GetEnumDescription(cancelStatus);
            }
            
        }

        public List<ApiRentPresentationStub> MapList(List<api_rent> dbItems)
        {
            List<ApiRentPresentationStub> retList = new List<ApiRentPresentationStub>();
            foreach (api_rent dbItem in dbItems)
                retList.Add(new ApiRentPresentationStub(dbItem));
            return retList;
        }
    }
}