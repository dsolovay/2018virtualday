using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;

namespace Demo1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            var cookie = Request.Cookies["visit_count"];
            int i;
            if (cookie == null)
            {
                Response.Cookies.Add(new HttpCookie("visit_count", "1"));
            } else if (Int32.TryParse(cookie.Value, out i))
            {
                Response.Cookies.Add(new HttpCookie("visit_count", (i+1).ToString()));
            }
      
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}