using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using test.Models;

namespace test.Controllers.Admin
{
    public class AdminController : Controller
    {
        public ActionResult Index()
        {

            if (Session["user"] != null) {
                Dictionary<string, string> user = JsonConvert.DeserializeObject<Dictionary<string, string>>(Session["user"].ToString());
                ViewBag.user = user;

                return View();
            }
            else
            {
                return RedirectToAction("index", "Login");
            }
            

        }
        public ActionResult First()//首页
        {
            //将session中以json类型存储的User
            //Dictionary<string, string> d = JsonConvert.DeserializeObject<Dictionary<string, string>>(Session["user"].ToString());

            //ViewBag.user = d;
            return View();
        }
        //
        //  员工信息
        //
        public ActionResult Personnel()//员工
        {
            //string staffs = GetPersonnel();
            //ViewBag.staffs = staffs;
            return View();
        }
        // public List<Staff> GetPersonnel()
        //显示员工简要信息
        public JsonResult GetPersonnelAjax()
        {
            string query = Request["query"];
            string department = Request["department"];
            int flag = 0;
            int flag_id = 0;
            int flag_department = 0;
            if (query == "")
            {
                flag_id = 1;
            }
            if (department == "0")
            {
                flag_department = 1;
            }
            if (query == "" && department == "0")
            {
                flag = 1;
            }

            List<Staff> staffs = new List<Staff>();

            string conn = ConfigurationManager.AppSettings["DBConn"];
            MySqlConnection con = new MySqlConnection(conn);
            con.Open();

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "select id,department,position,duty,type,name from staff where ((id=@id or @flag_id or name like @name)and (department=@department or @flag_department )) or @flag;";
            cmd.Parameters.AddWithValue("@id", query);
            cmd.Parameters.AddWithValue("@department", department);
            cmd.Parameters.AddWithValue("@flag_id", flag_id);
            cmd.Parameters.AddWithValue("@name", "%"+query+"%");
            cmd.Parameters.AddWithValue("@flag", flag);
            cmd.Parameters.AddWithValue("@flag_department", flag_department);


            cmd.Connection = con;

            MySqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Staff staff = new GeneralStaff();
                staff.Id = Convert.ToInt32(dr["id"].ToString());
                staff.Department = Convert.ToInt32(dr["department"].ToString());
                staff.Position = Convert.ToInt32(dr["position"].ToString());
                staff.Duty = Convert.ToInt32(dr["duty"].ToString());
                staff.Type = Convert.ToInt32(dr["type"].ToString());
                staff.Name = dr["name"].ToString();

                if(staff!=null)
                    staffs.Add(staff);
            }
            //string jsonStr = JsonConvert.SerializeObject(staffs);//将链表转成json字符串
            //JsonResult json = new JsonResult();
            //json
            dr.Close();
            con.Close();
            return Json(staffs, JsonRequestBehavior.AllowGet);
        }
        //添加新员工
        public ActionResult WriteStaff()
        {
            string name = Request["name"];
            int sex = Convert.ToInt32(Request["sex"]);
            string birthday = Request["birthday"];
            string identity_card = Request["identity_card"];
            string place = Request["place"];
            string nation = Request["nation"];
            int marital_status = Convert.ToInt32(Request["marital_status"]);
            string politics_status = Request["politics_status"];
            int department = Convert.ToInt32(Request["department"]);
            int position = Convert.ToInt32(Request["position"]);
            int duty = Convert.ToInt32(Request["duty"]);
            int type = Convert.ToInt32(Request["type"]);
            int my_fixed = Convert.ToInt32(Request["fixed"]);
            int alltsutomu = Convert.ToInt32(Request["alltsutomu"]);
            int target = 0;
            int reach_bonus = 0;
            int commission = 0;
            if (duty == 11)
            {
                target = Convert.ToInt32(Request["target"]);
                reach_bonus = Convert.ToInt32(Request["reach_bonus"]);
                commission = Convert.ToInt32(Request["commission"]);
            }

            string conn = ConfigurationManager.AppSettings["DBConn"];
            MySqlConnection con = new MySqlConnection(conn);
            con.Open();
            //SqlTransaction sqltra = con.BeginTransaction();//开始事务
            MySqlTransaction mysqltra = con.BeginTransaction();//开始事务

            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = con;
            cmd.Transaction = mysqltra;//执行SQL时
            try
            {
                cmd.CommandText = "insert into staff (department,position,duty,type,name,fixed,target,reach_bonus,commission,alltsutomu)" +
                    " values(@department,@position,@duty,@type,@name,@fixed,@target,@reach_bonus,@commission,@alltsutomu)";
                cmd.Parameters.AddWithValue("@department", department);
                cmd.Parameters.AddWithValue("@position", position);
                cmd.Parameters.AddWithValue("@duty", duty);
                cmd.Parameters.AddWithValue("@type", type);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@fixed", my_fixed);
                cmd.Parameters.AddWithValue("@target", target);
                cmd.Parameters.AddWithValue("@reach_bonus", reach_bonus);
                cmd.Parameters.AddWithValue("@commission", commission);
                cmd.Parameters.AddWithValue("@alltsutomu", alltsutomu);

                cmd.ExecuteNonQuery();

                cmd.CommandText = "SELECT @@IDENTITY as id;";
                MySqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                int staff_id = Convert.ToInt32(dr["id"].ToString());
                dr.Close();

                cmd.Parameters.Clear();
                cmd.CommandText = "insert into staff_detail (staff_id,sex,birthday,place,nation,marital_status,politics_status,identity_card)" +
                    "values(@staff_id,@sex,@birthday,@place,@nation,@marital_status,@politics_status,@identity_card)";
                cmd.Parameters.AddWithValue("@staff_id", staff_id);
                cmd.Parameters.AddWithValue("@sex", sex);
                cmd.Parameters.AddWithValue("@birthday", birthday);
                cmd.Parameters.AddWithValue("@place", place);
                cmd.Parameters.AddWithValue("@nation", nation);
                cmd.Parameters.AddWithValue("@marital_status", marital_status);
                cmd.Parameters.AddWithValue("@politics_status", politics_status);
                cmd.Parameters.AddWithValue("@identity_card", identity_card);
                cmd.ExecuteNonQuery();

                mysqltra.Commit();
            }
            catch (Exception)
            {
                mysqltra.Rollback();//出错回滚
            }



            ViewBag.name = name;
            ViewBag.sex = sex;
            ViewBag.birthday = birthday;
            ViewBag.identity_card = identity_card;
            ViewBag.place = place;
            ViewBag.nation = nation;
            ViewBag.marital_status = marital_status;
            ViewBag.politics_status = politics_status;
            ViewBag.department = department;
            ViewBag.position = position;
            ViewBag.duty = duty;
            ViewBag.type = type;
            ViewBag.my_fixed = my_fixed;
            ViewBag.alltsutomu = alltsutomu;
            ViewBag.target = target;
            ViewBag.reach_bonus = reach_bonus;
            ViewBag.commission = commission;



            return View("Personnel");
        }
        //返回详细信息
        public JsonResult GetStaffDetail()
        {
            string staff_id = Request["staff_id"];

            Dictionary<string, string> dic = new Dictionary<string, string>();

            string conn = ConfigurationManager.AppSettings["DBConn"];
            MySqlConnection con = new MySqlConnection(conn);
            con.Open();

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "select name,sex,birthday,place,nation,marital_status,politics_status,identity_card,staff.id,department,position,duty,type,fixed,sale,target,reach_bonus,commission,grade,alltsutomu " +
                "from staff left join staff_detail  on staff.id = staff_detail.staff_id where staff.id = @staff_id";
            cmd.Parameters.AddWithValue("@staff_id", staff_id);

            cmd.Connection = con;

            MySqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                dic.Add("name", dr["name"].ToString());
                dic.Add("sex", dr["sex"].ToString());
                dic.Add("birthday", dr["birthday"].ToString());
                dic.Add("place", dr["place"].ToString());
                dic.Add("nation", dr["nation"].ToString());
                dic.Add("marital_status", dr["marital_status"].ToString());
                dic.Add("politics_status", dr["politics_status"].ToString());
                dic.Add("identity_card", dr["identity_card"].ToString());
                dic.Add("id", dr["id"].ToString());
                dic.Add("department", dr["department"].ToString());
                dic.Add("position", dr["position"].ToString());
                dic.Add("duty", dr["duty"].ToString());
                dic.Add("type", dr["type"].ToString());
                dic.Add("fixed", dr["fixed"].ToString());
                dic.Add("sale", dr["sale"].ToString());
                dic.Add("target", dr["target"].ToString());
                dic.Add("reach_bonus", dr["reach_bonus"].ToString());
                dic.Add("commission", dr["commission"].ToString());
                dic.Add("grade", dr["grade"].ToString());
                dic.Add("alltsutomu", dr["alltsutomu"].ToString());
            }
            dr.Close();
            con.Close();


            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        //
        //工资表
        //
        public ActionResult Salary()
        {
            return View();
        }
        public JsonResult GetSalaryAjax()
        {
            string query = Request["query"];
            string year = Request["year"];
            string month = Request["month"];
            int flag_year = 0;
            int flag_query = 0;
            int flag_month = 0;
            if (query == "")
            {
                flag_query = 1;
            }
            if (year == "")
            {
                flag_year = 1;
            }
            if (month == "")
            {
                flag_month = 1;
            }

            List<Salary> salaries = new List<Salary>();

            string conn = ConfigurationManager.AppSettings["DBConn"];
            MySqlConnection con = new MySqlConnection(conn);
            con.Open();

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "select staff.id,name,department,salary.fixed,year,month,overtime,leavemoney,late,extra,reachbonus,technical,alltsutomued,total" +
                " from staff,salary where staff.id=salary.staff_id and (staff.id = @id or @flag_query or name like @name  ) and (year = @year or @flag_year) and ( month = @month or @flag_month)";
            //员工ID、姓名、部门、固定薪资、年、月、加班工资、请假扣除工资、迟到扣除工资、抽成、销售指标达成奖金、技术津贴、全勤奖金、总工资

            cmd.Parameters.AddWithValue("@id",query);
            cmd.Parameters.AddWithValue("@name","%"+query+"%");
            cmd.Parameters.AddWithValue("@flag_query",flag_query);
            cmd.Parameters.AddWithValue("@year",year);
            cmd.Parameters.AddWithValue("@flag_year",flag_year);
            cmd.Parameters.AddWithValue("@month",month);
            cmd.Parameters.AddWithValue("@flag_month",flag_month);

            cmd.Connection = con;

            MySqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Salary salary = new Salary();
                salary.Staff_id = Convert.ToInt32(dr["id"].ToString());
                salary.Staff_name = dr["name"].ToString();
                salary.Department = Convert.ToInt32(dr["department"].ToString());
                salary.Fixed = Convert.ToInt32(dr["fixed"].ToString());
                salary.Year = Convert.ToInt32(dr["year"].ToString());
                salary.Month = Convert.ToInt32(dr["month"].ToString());
                salary.Overtime = Convert.ToInt32(dr["overtime"].ToString());
                salary.Leave = Convert.ToInt32(dr["leavemoney"].ToString());
                salary.Late = Convert.ToInt32(dr["late"].ToString());
                salary.Extra = Convert.ToInt32(dr["extra"].ToString());
                salary.ReachBonus = Convert.ToInt32(dr["reachbonus"].ToString());
                salary.TechnicalAllowance = Convert.ToInt32(dr["technical"].ToString());
                salary.AllTsutomu = Convert.ToInt32(dr["alltsutomued"].ToString());
                salary.Total = Convert.ToInt32(dr["total"].ToString());

                salaries.Add(salary);
               
            }
            //string jsonStr = JsonConvert.SerializeObject(staffs);//将链表转成json字符串
            //JsonResult json = new JsonResult();
            //json
            dr.Close();
            con.Close();
            return Json(salaries, JsonRequestBehavior.AllowGet);
        }
        //工资计算页面
        public ActionResult CountSalary()
        {
            return View();
        }
        //计算工资并添加到数据库
        public JsonResult CountSalaryAjax()
        {
            int year = Convert.ToInt32(Request["year"].Trim());
            int month = Convert.ToInt32(Request["month"].Trim());

            List<GeneralStaff> generalStaffs = new List<GeneralStaff>();
            List<Salary> salaries = new List<Salary>();

            string conn = ConfigurationManager.AppSettings["DBConn"];
            MySqlConnection con = new MySqlConnection(conn);
            con.Open();

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "select id,department,position,duty,type,name,month_day,grade,sale,target,reach_bonus,commission,fixed,alltsutomu from staff";

            cmd.Connection = con;

            MySqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                GeneralStaff general = null;
                if (dr["position"].ToString().Trim() == "3")
                {
                    Technician technician = new Technician();
                    technician.Grade = Convert.ToInt32(dr["grade"].ToString());

                    general = technician;
                }
                else if(dr["position"].ToString().Trim() == "2")
                {
                    Salesman salesman = new Salesman();
                    salesman.Sale = Convert.ToInt32(dr["sale"].ToString());
                    salesman.Target = Convert.ToInt32(dr["target"].ToString());
                    salesman.ReachBonus = Convert.ToInt32(dr["reach_bonus"].ToString());
                    salesman.Commission = Convert.ToInt32(dr["commission"].ToString());
                    general = salesman;
                }else
                {
                    GeneralStaff generalStaff = new GeneralStaff();
                    general = generalStaff;
                }
                general.Id = Convert.ToInt32(dr["id"].ToString());
                general.Department = Convert.ToInt32(dr["department"].ToString());
                general.Position = Convert.ToInt32(dr["position"].ToString());
                general.Duty = Convert.ToInt32(dr["duty"].ToString());
                general.Type = Convert.ToInt32(dr["type"].ToString());
                general.Name = dr["name"].ToString();
                general.MonthTotalDays = Convert.ToInt32(dr["month_day"].ToString());
                general.Fixed = Convert.ToInt32(dr["Fixed"].ToString());
                general.AllTsutomu = Convert.ToInt32(dr["alltsutomu"].ToString());

                general.Absence = 0;
                general.SickLeave =0;
                general.Lates =0;
                general.OvertimeDay = 2;

                generalStaffs.Add(general);
            }

            dr.Close();

            foreach (GeneralStaff g in generalStaffs)
            {
                Salary salary = g.GetSalary(year,month);
                salaries.Add(salary);
            }

            foreach(Salary salary in salaries)
            {
                cmd.CommandText = "insert into salary (staff_id,year,month,total,overtime,leavemoney,late,extra,reachbonus,alltsutomued,technical,fixed) " +
                    "values(@staff_id,@year,@month,@total,@overtime,@leavemoney,@late,@extra,@reachbonus,@alltsutomued,@technical,@fixed)";

                cmd.Parameters.Clear();

                cmd.Parameters.AddWithValue("@staff_id", salary.Staff_id);
                cmd.Parameters.AddWithValue("@year", salary.Year);
                cmd.Parameters.AddWithValue("@month", salary.Month);
                //cmd.Parameters.AddWithValue("@total", 123456);
                cmd.Parameters.AddWithValue("@total", salary.Total);
                cmd.Parameters.AddWithValue("@overtime", salary.Overtime);
                cmd.Parameters.AddWithValue("@leavemoney", salary.Leave);
                cmd.Parameters.AddWithValue("@late", salary.Late);
                cmd.Parameters.AddWithValue("@extra", salary.Extra);
                cmd.Parameters.AddWithValue("@reachbonus", salary.ReachBonus);
                cmd.Parameters.AddWithValue("@alltsutomued", salary.AllTsutomu);
                cmd.Parameters.AddWithValue("@technical", salary.TechnicalAllowance);
                cmd.Parameters.AddWithValue("@fixed", salary.Fixed);

                //cmd.Connection = con;

                cmd.ExecuteNonQuery();

                
            }
            con.Close();
            
            //string jsonStr = JsonConvert.SerializeObject(salaries);
            //return jsonStr;
            return Json(salaries, JsonRequestBehavior.AllowGet);
        }
        
        /* public JsonResult Overtime()
        {
            DateTime dt = Convert.ToDateTime("2019-4-29");
            DateTime dt2 = Convert.ToDateTime("2019-4-30");
            int days = (dt2 - dt).Days;
            DateTime initial = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            string str = (dt2 - ts).TotalSeconds.ToString();

            DateTime test = ts.AddSeconds(1556582400);
            string s = test.ToString("yyyy-MM-dd");
            return Json(s, JsonRequestBehavior.AllowGet);
        }*/



        //
        //加班时间表
        //
        public ActionResult Overtime()
        {
            return View();
        }
        public JsonResult GetOvertime()
        {
            /*DateTime dt = Convert.ToDateTime("2019-4-30");
            DateTime dt2 = Convert.ToDateTime("2019-5-30");
            int days = (dt2 - dt).Days;
            return Json(days, JsonRequestBehavior.AllowGet);*/
            DateTime initial = new DateTime(1970,1,1,0,0,0,0);

            List<Overtime> overtimes = new List<Overtime>();

            string query = Request["query"];
            string department = Request["department"];
            string start = Request["start_date"];
            string end = Request["end_date"];
            int flag_query = 0;
            int flag_end = 0;
            int flag_start = 0;
            string start_date = null;
            string end_date = null;
            if (query == "" || query == null)
            {
                flag_query = 1;
            }
            if(start == "" || start == null)
            {
                flag_start = 1;
            }
            else
            {
                start_date = (Convert.ToDateTime(start) - initial).TotalSeconds.ToString();
            }
            if (end == "" || end == null)
            {
                flag_end = 1;
            }
            else
            {
                end_date = (Convert.ToDateTime(end) - initial).TotalSeconds.ToString();
            }


            string conn = ConfigurationManager.AppSettings["DBConn"];
            MySqlConnection con = new MySqlConnection(conn);
            con.Open();

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "select staff.id,name,overtime.id as overtime_id,end_date,start_date,duration,overtime.type,explains from overtime,staff where staff.id = overtime.staff_id and" +
                " status = '1' and (@flag_query or staff.id = @id or name like @name ) and (@flag_start or start_date >= @start_date) and (@flag_end or end_date <= @end_date)";

            cmd.Parameters.AddWithValue("@id", query);
            cmd.Parameters.AddWithValue("@name", "%"+query+"%");
            cmd.Parameters.AddWithValue("@flag_query", flag_query);
            cmd.Parameters.AddWithValue("@flag_start", flag_start);
            cmd.Parameters.AddWithValue("@start_date", start_date);
            cmd.Parameters.AddWithValue("@flag_end", flag_end);
            cmd.Parameters.AddWithValue("@end_date", end_date);


            cmd.Connection = con;

            MySqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Overtime overtime = new Overtime();
                overtime.Staff_id = Convert.ToInt32(dr["id"].ToString());
                overtime.Name = dr["name"].ToString();
                /*overtime.Start_time = dr["start_time"].ToString();
                overtime.End_time = dr["end_time"].ToString();*/
                overtime.Start_date = initial.AddSeconds(Convert.ToInt32(dr["start_date"].ToString())).ToString("yyyy-MM-dd HH:mm:ss");
                overtime.End_date = initial.AddSeconds(Convert.ToInt32(dr["end_date"].ToString())).ToString("yyyy-MM-dd HH:mm:ss");
                overtime.Duration = Convert.ToDouble(dr["duration"].ToString());
                overtime.Type = Convert.ToInt32(dr["type"].ToString());
                overtime.Explains = dr["explains"].ToString();
                overtime.Id = Convert.ToInt32(dr["overtime_id"].ToString());

                overtimes.Add(overtime);
            }
            dr.Close();
            con.Close();
            return Json(overtimes, JsonRequestBehavior.AllowGet);
        }
        //添加加班信息
        public JsonResult WriteOvertime()
        {
            DateTime initial = new DateTime(1970, 1, 1, 0, 0, 0, 0);

            string staff_id = Request["staff_id"];
            string write_start = Request["write_start"];
            string write_end = Request["write_end"];
            string duration = Request["duration"];
            string type = Request["type"];
            string explains = Request["explains"];
            string start_time = null;
            string end_time = null;
            start_time = (Convert.ToDateTime(write_start) - initial).TotalSeconds.ToString();
            end_time = (Convert.ToDateTime(write_end) - initial).TotalSeconds.ToString();
            if (explains == null)
            {
                explains = "";
            }

            string conn = ConfigurationManager.AppSettings["DBConn"];
            MySqlConnection con = new MySqlConnection(conn);
            con.Open();

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "INSERT overtime(staff_id,start_date,end_date,duration,type,explains)" +
                              " values(@staff_id,@start_date,@end_date,@duration,@type,@explains)";
            cmd.Parameters.AddWithValue("@staff_id", staff_id);
            cmd.Parameters.AddWithValue("@start_date", start_time);
            cmd.Parameters.AddWithValue("@end_date", end_time);
            cmd.Parameters.AddWithValue("@duration", duration);
            cmd.Parameters.AddWithValue("@type", type);
            cmd.Parameters.AddWithValue("@explains", explains);
            cmd.Connection = con;

            cmd.ExecuteNonQuery();

            con.Close();

            return Json(true, JsonRequestBehavior.AllowGet);
        }
        //删除加班信息
        public JsonResult DeleteOvertime() {

            int flag = 0;
            string id = Request["overtime_id"];
            string conn = ConfigurationManager.AppSettings["DBConn"];
            MySqlConnection con = new MySqlConnection(conn);
            con.Open();

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "update overtime set status = @status where id = @id";
            cmd.Parameters.AddWithValue("@status", 2);
            cmd.Parameters.AddWithValue("@id", id);

            cmd.Connection = con;

            flag = cmd.ExecuteNonQuery();

            con.Close();
            if(flag>0)
                return Json(true, JsonRequestBehavior.AllowGet);
            else
                return Json(false, JsonRequestBehavior.AllowGet);
        }


        //
        //休假
        //
        public ActionResult Leave()
        {
            return View();
        }
        public JsonResult GetLeave()
        {
            /*DateTime dt = Convert.ToDateTime("2019-4-30");
            DateTime dt2 = Convert.ToDateTime("2019-5-30");
            int days = (dt2 - dt).Days;
            return Json(days, JsonRequestBehavior.AllowGet);*/
            DateTime initial = new DateTime(1970, 1, 1, 0, 0, 0, 0);

            List<Leave> leaves = new List<Leave>();

            string query = Request["query"];
            string department = Request["department"];
            string start = Request["start_date"];
            string end = Request["end_date"];
            int flag_query = 0;
            int flag_end = 0;
            int flag_start = 0;
            string start_time = null;
            string end_time = null;
            if (query == "" || query == null)
            {
                flag_query = 1;
            }
            if (start == "" || start == null)
            {
                flag_start = 1;
            }
            else
            {
                start_time = (Convert.ToDateTime(start) - initial).TotalSeconds.ToString();
            }
            if (end == "" || end == null)
            {
                flag_end = 1;
            }
            else
            {
                end_time = (Convert.ToDateTime(end) - initial).TotalSeconds.ToString();
            }


            string conn = ConfigurationManager.AppSettings["DBConn"];
            MySqlConnection con = new MySqlConnection(conn);
            con.Open();

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "select staff.id,name,leaved.id as leave_id ,start_time,end_time,days,leaved.type,explains from leaved,staff where staff.id = leaved.staff_id and" +
                " status = '1' and (@flag_query or staff.id = @id or name like @name ) and (@flag_start or start_time >= @start_time) and (@flag_end or end_time <= @end_time)";

            cmd.Parameters.AddWithValue("@id", query);
            cmd.Parameters.AddWithValue("@name", "%" + query + "%");
            cmd.Parameters.AddWithValue("@flag_query", flag_query);
            cmd.Parameters.AddWithValue("@flag_start", flag_start);
            cmd.Parameters.AddWithValue("@start_time", start_time);
            cmd.Parameters.AddWithValue("@flag_end", flag_end);
            cmd.Parameters.AddWithValue("@end_time", end_time);


            cmd.Connection = con;

            MySqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Leave leave = new Leave();
                leave.Staff_id = Convert.ToInt32(dr["id"].ToString());
                leave.Name = dr["name"].ToString();
                /*leave.Start_time = dr["start_time"].ToString();
                leave.End_time = dr["end_time"].ToString();*/
                leave.Start_time = initial.AddSeconds(Convert.ToInt32(dr["start_time"].ToString())).ToString("yyyy-MM-dd");
                leave.End_time = initial.AddSeconds(Convert.ToInt32(dr["end_time"].ToString())).ToString("yyyy-MM-dd");

                leave.Days = Convert.ToInt32(dr["days"].ToString());
                leave.Type = Convert.ToInt32(dr["type"].ToString());
                leave.Explains = dr["explains"].ToString();
                leave.Id = Convert.ToInt32(dr["leave_id"].ToString());

                leaves.Add(leave);
            }
            dr.Close();
            con.Close();
            return Json(leaves, JsonRequestBehavior.AllowGet);
        }
        //添加休假信息
        public JsonResult WriteLeave()
        {
            DateTime initial = new DateTime(1970, 1, 1, 0, 0, 0, 0);

            string staff_id = Request["staff_id"];
            string write_start = Request["write_start"];
            string write_end = Request["write_end"];
            string days = Request["days"];
            string type = Request["type"];
            string explains = Request["explains"];
            string start_time = null;
            string end_time = null;
            start_time = (Convert.ToDateTime(write_start) - initial).TotalSeconds.ToString();
            end_time = (Convert.ToDateTime(write_end) - initial).TotalSeconds.ToString();
            if (explains == null)
            {
                explains = "";
            }

            string conn = ConfigurationManager.AppSettings["DBConn"];
            MySqlConnection con = new MySqlConnection(conn);
            con.Open();

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "INSERT leaved(staff_id,start_time,end_time,days,type,explains)" +
                              " values(@staff_id,@start_date,@end_date,@days,@type,@explains)";
            cmd.Parameters.AddWithValue("@staff_id", staff_id);
            cmd.Parameters.AddWithValue("@start_date", start_time);
            cmd.Parameters.AddWithValue("@end_date", end_time);
            cmd.Parameters.AddWithValue("@days", days);
            cmd.Parameters.AddWithValue("@type", type);
            cmd.Parameters.AddWithValue("@explains", explains);
            cmd.Connection = con;

            cmd.ExecuteNonQuery();

            return Json(true, JsonRequestBehavior.AllowGet);
        }
        public JsonResult DeleteLeave() {

            int flag = 0;
            string id = Request["levae_id"];
            string conn = ConfigurationManager.AppSettings["DBConn"];
            MySqlConnection con = new MySqlConnection(conn);
            con.Open();

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "update leaved set status = @status where id = @id";
            cmd.Parameters.AddWithValue("@status", 2);
            cmd.Parameters.AddWithValue("@id", id);

            cmd.Connection = con;

            flag = cmd.ExecuteNonQuery();

            con.Close();
            if(flag>0)
                return Json(true, JsonRequestBehavior.AllowGet);
            else
                return Json(false, JsonRequestBehavior.AllowGet);
        }




        //
        //小模块
        //提供用户选择列表
        //
        public JsonResult SelectStaffAjax()
        {
            string query = Request["query"];
            string department = Request["department"];
            string[] departments = ConfigurationManager.AppSettings["Departments"].Split(';');
            string[] positions = ConfigurationManager.AppSettings["Positions"].Split(';');
            string[] dutys = ConfigurationManager.AppSettings["Dutys"].Split(';');
            int flag = 0;
            int flag_id = 0;
            int flag_department = 0;
            if (query == "")
            {
                flag_id = 1;
            }
            if (department == "0")
            {
                flag_department = 1;
            }
            if (query == "" && department == "0")
            {
                flag = 1;
            }

            string conn = ConfigurationManager.AppSettings["DBConn"];
            MySqlConnection con = new MySqlConnection(conn);
            con.Open();

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "select id,department,name,position,duty from staff where ((id=@id or @flag_id or name like @name)and (department=@department or @flag_department )) or @flag;";
            cmd.Parameters.AddWithValue("@id", query);
            cmd.Parameters.AddWithValue("@department", department);
            cmd.Parameters.AddWithValue("@flag_id", flag_id);
            cmd.Parameters.AddWithValue("@name", "%" + query + "%");
            cmd.Parameters.AddWithValue("@flag", flag);
            cmd.Parameters.AddWithValue("@flag_department", flag_department);
            cmd.Connection = con;
            List<Dictionary<string, string>> list = new List<Dictionary<string, string>>();
               
            MySqlDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                Dictionary<string, string> d = new Dictionary<string, string>();
                d.Add("name", dr["name"].ToString());
                d.Add("id", dr["id"].ToString());
                d.Add("department", departments[Convert.ToInt32(dr["department"].ToString())]);
                d.Add("position", positions[Convert.ToInt32(dr["position"].ToString())]);
                d.Add("duty", dutys[Convert.ToInt32(dr["duty"].ToString())]);
                list.Add(d);
            }
            dr.Close();
            con.Close();

            
            return Json(list, JsonRequestBehavior.AllowGet);
        }

        //计算时数
        public JsonResult GetDurationAjax()
        {
            Dictionary<string, double> dic = new Dictionary<string, double>();
            string write_start = Request["starttime"];
            string write_end = Request["endtime"];
            if (write_end == "" ||write_start == "")
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            double seconds = (Convert.ToDateTime(write_end) - Convert.ToDateTime(write_start)).TotalSeconds;
            double duration = seconds / 3600.0;
            dic.Add("duration", duration);

            return Json(dic, JsonRequestBehavior.AllowGet);
        }

        //计算天数 还没修改
        public JsonResult GetDaysAjax()
        {
            Dictionary<string, double> dic = new Dictionary<string, double>();
            string write_start = Request["starttime"];
            string write_end = Request["endtime"];
            if (write_end == "" || write_start == "")
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
            double seconds = (Convert.ToDateTime(write_end) - Convert.ToDateTime(write_start)).TotalSeconds;
            double days = seconds / 3600.0;
            dic.Add("days", days);

            return Json(dic, JsonRequestBehavior.AllowGet);
        }
        
        public void SendEmailAjax()
        {
            string email = Request["email"];
            SmtpClient client = new SmtpClient("smtp.163.com", 25);//("邮箱服务器类型", 端口号) 网易163邮箱的端口号为25 QQ邮箱的端口号为587
            Random Rdm = new Random();
            int iRdm = Rdm.Next(0, 999999);//生成随机数
            string code = iRdm.ToString().PadLeft(5, '0');//随机数前空位补0
            string htmlmessgae = "<div><table>"+
                                 "<tr><td style='padding:0; font-family:Microsoft Yahei, Verdana, Simsun, sans-serif; font-size:17px; color:#707070;'>**物流系统账户</td></tr>" +
                                 "<tr><td style='padding:0; font-family:Microsoft Yahei, Verdana, Simsun, sans-serif; font-size:41px; color:#2672ec;'>密码重置代码</td></tr>" +
                                 "<tr><td style='padding:0; padding-top:25px; font-family:Microsoft Yahei, Verdana, Simsun, sans-serif; font-size:14px; color:#2a2a2a;'>请使用此代码为 物流系统账户 帐户 " + email + " 重置密码。</td></tr>" +
                                 "<tr><td style='padding:0; padding-top:25px; font-family:Microsoft Yahei, Verdana, Simsun, sans-serif; font-size:14px; color:#2a2a2a;'>你的代码如下：<span style='border-bottom: 1px dashed rgb(204, 204, 204); z-index: 1; position: static; font-weight:bolder'>" + code+ "</span></td></tr>" +
                                 "<tr><td style='padding:0; padding-top:25px; font-family:Microsoft Yahei, Verdana, Simsun, sans-serif; font-size:14px; color:#2a2a2a;'>谢谢！</td></tr>" +
                                 "<tr><td style='padding:0; font-family:Microsoft Yahei, Verdana, Simsun, sans-serif; font-size:14px; color:#2a2a2a;'>物流系统 帐户团队</td></tr>" +
                                 "</table></div>";

            MailMessage msg = new MailMessage("lgztestasp@163.com", email, "验证码", htmlmessgae);//邮件信息 MailMessage（发件人邮箱，收件人邮箱，邮件主体，邮件内容）；
            msg.IsBodyHtml = true;//邮件是否是html格式
            client.UseDefaultCredentials = false;
            NetworkCredential basicAuthenticationInfo =new NetworkCredential("lgztestasp@163.com", "lgz123456");//证书（网络凭据） NetworkCredential（发件人邮箱，SMTP授权密码）
            client.Credentials = basicAuthenticationInfo;
            client.EnableSsl = true;
            
            client.Send(msg);
        }
        /*public List<Menu> GetMenus()
        {
            string conn = ConfigurationManager.AppSettings["DBConn"];
            MySqlConnection con = new MySqlConnection(conn);
            con.Open();

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "select * from admin_menu where status = 1";

            cmd.Connection = con;

            MySqlDataReader dr = cmd.ExecuteReader();

            List<Menu> menus = new List<Menu>();

            while (dr.Read())
            {
                Menu menu = new Menu();

                menu.Id = Convert.ToInt32(dr["id"]);
                menu.Pid = Convert.ToInt32(dr["pid"]);
                menu.Sort = Convert.ToInt32(dr["sort"]);
                menu.Name = dr["name"].ToString();
                menu.Path = dr["path"].ToString();
                menu.Icon = dr["icon"].ToString();

                menus.Add(menu);
            }



            dr.Close();
            con.Close();

            return menus;
        }*/

    }
}