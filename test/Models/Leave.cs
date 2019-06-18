using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace test.Models
{
    public class Leave
    {
        public int Id { get; set; }
        public int Staff_id { get; set; }
        public string Name { get; set; }
        public string Start_time { get; set; }
        public string End_time { get; set; }
        public int Days { get; set; }
        public int Type { get; set; }
        public string Explains { get; set; }
    }
}