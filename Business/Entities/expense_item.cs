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
    
    public partial class expense_item
    {
        public System.Guid id { get; set; }
        public System.Guid id_expense { get; set; }
        public string category { get; set; }
        public int value { get; set; }
        public string description { get; set; }
    
        public virtual expense expense { get; set; }
    }
}
