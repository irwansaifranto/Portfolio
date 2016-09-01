using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Entities
{
    public class log
    {
        public long id { get; set; }
        public System.DateTime timestamp { get; set; }
        public string application { get; set; }
        public string ip { get; set; }
        public string user { get; set; }
        public string action { get; set; }
        public string data { get; set; }
    }
}
