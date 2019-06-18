using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace test.Models
{
    //加班类
    public class Overtime
    {
        public int Id { get; set; }
        public int Staff_id { get; set; }
        public string Name { get; set; }
        public string Start_date { get; set; }
        public string End_date { get; set; }
        public double Duration { get; set; }
        public int Type { get; set; }
        public string Explains { get; set; }
    }
}