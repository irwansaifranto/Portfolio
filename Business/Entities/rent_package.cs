//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Business.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class rent_package
    {
        public System.Guid id_rent { get; set; }
        public System.Guid id_car_package { get; set; }
        public int quantity { get; set; }
        public int price_each { get; set; }
        public System.Guid id { get; set; }
    
        public virtual car_package car_package { get; set; }
        public virtual rent rent { get; set; }
    }
}