using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace test.Controllers.Public
{
    public class LoginOutController : Controller
    {
        // GET: LoginOut
        public ActionResult Index()
        {
            Session.RemoveAll();

            return RedirectToAction("index", "Login");
        }
    }
}