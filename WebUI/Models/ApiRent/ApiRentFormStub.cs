using Business.Entities;
using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebUI.Models.ApiRent
{
    public class ApiRentFormStub
    {
        public Guid IdRent { get; set; }
        public string Partner { get; set; }
        public ApiRentStatus Status { get; set; }
        public ApiRentStatus? CancellationStatus { get; set; }

        public ApiRentFormStub() { }

        public void SetNewRent(rent dbItem, string partner)
        {
            IdRent = dbItem.id;
            Status = ApiRentStatus.NEW;
            Partner = partner;
        }

        public api_rent GetDbObjectOnCreate(string partner) 
        {
            api_rent dbItem = new api_rent
            {
                id_rent = IdRent,
                partner = Partner,
                status = Status.ToString()
            };

            if (CancellationStatus.HasValue)
                dbItem.cancellation_status = CancellationStatus.Value.ToString();
            
            return dbItem;
        }
    }
}