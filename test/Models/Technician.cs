using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace test.Models
{
    //技术员
    public class Technician : GeneralStaff
    {
        public static int[] Gradesubsidy = { 500, 1000,2000 };
        public int Grade { get; set; }//技术等级

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
            if (this.Absence + this.SickLeave + this.Lates == 0)
            {
                salary.AllTsutomu += this.AllTsutomu;
            };
            salary.TechnicalAllowance = Gradesubsidy[this.Grade];
            salary.SetTotal();
            return salary;
        }
    }
}