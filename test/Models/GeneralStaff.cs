using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace test.Models
{
    //一般员工 除销售人员与技术员
    public class GeneralStaff : Staff
    {
        public int Absence { get; set; }//事假天数
        public int SickLeave { get; set; }//病假天数
        public int Lates { get; set; }//迟到次数
        public int OvertimeDay { get; set; }//加班天数
        public int MonthTotalDays { get; set; }//月计薪天数
        public override Salary GetSalary(int year ,int month)
        {
            Salary salary = new Salary();
            salary.Staff_id = Id;
            salary.Year = year;
            salary.Month = month;
            salary.Staff_name = Name;
            salary.Department = Department;
            salary.Fixed = Fixed;
            salary.Overtime = OvertimeDay * (Fixed / MonthTotalDays);
            salary.Leave = (int)(Absence + SickLeave*0.5)* (Fixed / MonthTotalDays);
            salary.Late = Lates * 2000;
            salary.AllTsutomu = 0;
            if (Absence+SickLeave+Lates == 0)
            {
                salary.AllTsutomu += AllTsutomu;
            };
            salary.SetTotal();
            return salary;
        }
    }
}