using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace test.Models
{
    public abstract class  Staff
    {
        public string Name { get; set; }//姓名
        public int Id { get; set; }//工号
        public int Department { get; set; }//部门
        public int Position { get; set; }//岗位
        public int Duty { get; set; }//职务
        public int Type { get; set; }//工种
        public int Fixed { get; set; }//固定工资 存的是整型 实际工资除100

        public int AllTsutomu { get; set; }//全勤奖金
        public abstract Salary GetSalary(int year, int month);//计算工资
    }
}