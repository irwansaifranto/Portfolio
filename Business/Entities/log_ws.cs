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
    
    public partial class log_ws
    {
        public System.DateTimeOffset created_time { get; set; }
        public string request_body { get; set; }
        public string response_body { get; set; }
        public string url { get; set; }
        public string request_header { get; set; }
        public string response_header { get; set; }
        public System.Guid id { get; set; }
    }
}