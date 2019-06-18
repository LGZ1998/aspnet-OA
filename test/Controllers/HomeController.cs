using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace test.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            Dictionary<string, string> d = new Dictionary<string, string>();
            d.Add("id", "1");
            d.Add("name", "lgz");

            string jsonStr = JsonConvert.SerializeObject(d);

            ViewBag.Message = jsonStr;

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult Hello()
        {
            ViewBag.name = "lgz";
            return View();
        }
        public ActionResult Test()
        {
            /*string conn = ConfigurationManager.AppSettings["DBConn"];
            MySqlConnection con = new MySqlConnection(conn);
            con.Open();

            MySqlCommand cmd = new MySqlCommand("select * from plan", con);
            MySqlDataReader dr = cmd.ExecuteReader();

            dr.Read();
            ViewBag.res = dr["content"];
            dr.Close();
            con.Close();

            return View();*/
            string conn = ConfigurationManager.AppSettings["DBConn"];
            MySqlConnection con = new MySqlConnection(conn);
            con.Open();

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "select * from plan where name = @name";
            cmd.Parameters.AddWithValue("@name", "休闲生活");

            cmd.Connection = con;

            MySqlDataReader dr = cmd.ExecuteReader();
            dr.Read();
            ViewBag.res = dr["content"];
            dr.Close();
            con.Close();

            return View();
        }
    }
}