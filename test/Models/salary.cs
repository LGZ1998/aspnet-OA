using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace test.Models
{
    //工资类
    public class Salary
    {
        public int Staff_id { get; set; }//员工ID
        public string Staff_name { get; set; }//员工姓名
        public int Department { get; set; }//部门
        public int Fixed { get; set; }//固定工资
        public int Year { get; set; }//年
        public int Month { get; set; }//月
        public int Overtime { get; set; }//加班
        public int Leave { get; set; }//请假
        public int Late { get; set; }//迟到
        public int Total { get; set; }//最终工资
        public int Extra;//抽成
        public int ReachBonus;//指标达成奖金
        public int AllTsutomu { get; set; }//全勤奖金
        public int TechnicalAllowance { get; set; }//技术津贴
        public void SetTotal()
        {
            this.Total = this.Fixed + Overtime - Leave - Late+Extra+ReachBonus+AllTsutomu+ TechnicalAllowance;
        }
    }
}