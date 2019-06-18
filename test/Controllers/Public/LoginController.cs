using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace test.Controllers.Public
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public string UserLoginAjax()
        {
            string name = Request["name"];
            string password = MD5Encrypt32(Request["pwd"]);

            string conn = ConfigurationManager.AppSettings["DBConn"];
            MySqlConnection con = new MySqlConnection(conn);
            con.Open();

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "select id,nickname from user where username = @username and password = @password;";
            cmd.Parameters.AddWithValue("@username", name);
            cmd.Parameters.AddWithValue("@password", password);

            cmd.Connection = con;

            Dictionary<string, string> d = new Dictionary<string, string>();

            MySqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {

                //Session["id"] = dr["id"];
                //Session["name"] = dr["nickname"];


                d.Add("id", dr["id"].ToString());
                d.Add("name", dr["nickname"].ToString());

               

                dr.Close();
                con.Close();

            }
            else
            {
                dr.Close();
                con.Close();
            }
            string jsonStr = JsonConvert.SerializeObject(d);
            Session["user"] = jsonStr;

            return jsonStr;


        }
        [HttpPost]
        public string AdminLoginAjax()
        {
            string name = Request["name"];
            string password = MD5Encrypt32(Request["pwd"]);

            string conn = ConfigurationManager.AppSettings["DBConn"];
            MySqlConnection con = new MySqlConnection(conn);
            con.Open();

            MySqlCommand cmd = new MySqlCommand();
            cmd.CommandText = "select id,nickname from adminuser where username = @username and password = @password;";
            //预定义SQL语句
            cmd.Parameters.AddWithValue("@username", name);
            cmd.Parameters.AddWithValue("@password", password);

            cmd.Connection = con;

            Dictionary<string, string> d = new Dictionary<string, string>();

            MySqlDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                //Session["id"]   = dr["id"];
                //Session["name"] = dr["nickname"];


                d.Add("id", dr["id"].ToString());
                d.Add("name", dr["nickname"].ToString());


                dr.Close();
                con.Close();

            }
            else
            {
                dr.Close();
                con.Close();
            }
            string jsonStr = JsonConvert.SerializeObject(d);
            Session["user"] = jsonStr;

            return jsonStr;

            
        }
        //MD5加密
        public static string MD5Encrypt32(string password)
        {
            string str = password;
            MD5 md5 = new MD5CryptoServiceProvider();

            byte[] fromData = Encoding.UTF8.GetBytes(str);
            byte[] targetData = md5.ComputeHash(fromData);

            string pwd = "";
            for(int i=0 ; i < targetData.Length; i++)
            {
                pwd += targetData[i].ToString("x2");
            }

            return pwd;
        }
    }
}