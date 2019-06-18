using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace test.Models
{
    //销售员
    public class Salesman: GeneralStaff
    {
        public int Sale;//销售额
        public int Target;//目标销售额
        public int ReachBonus;//指标达成奖金
        public int Commission;//销售额抽成千分比

        public override Salary GetSalary(int year, int month)
        {
            Salary salary = new Salary();
            salary.Staff_id = this.Id;
            salary.Year = year;
            salary.Month = month;
            salary.Staff_name = this.Name;
            salary.Department = this.Department;
            salary.Fixed = this.Fixed;
            salary.Overtime = this.OvertimeDay * (this.Fixed / this.MonthTotalDays);
            salary.Leave = (int)(this.Absence + this.SickLeave * 0.5) * (this.Fixed / this.MonthTotalDays);
            salary.Late = this.Lates * 2000;
            salary.AllTsutomu = 0;
            salary.ReachBonus = 0;
            if (this.Absence + this.SickLeave + this.Lates == 0)
            {
                salary.AllTsutomu += this.AllTsutomu;
            };
            if(this.Sale>= this.Target)
            {
                salary.ReachBonus += ReachBonus;
            }
            salary.Extra = this.Sale * Commission / 1000;
            
            salary.SetTotal();
            return salary;
        }
    }
}