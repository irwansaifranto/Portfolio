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
    
    public partial class car_brand
    {
        public car_brand()
        {
            this.car_model = new HashSet<car_model>();
        }
    
        public System.Guid id { get; set; }
        public string name { get; set; }
    
        public virtual ICollection<car_model> car_model { get; set; }
    }
}
