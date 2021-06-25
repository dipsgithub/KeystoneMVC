using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OpenAthensKeystoneDotNet4Sample.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {           
            Response.Redirect("/keystone/Account/Login");
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";
            Response.Redirect("/keystone/Account/Login");
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            Response.Redirect("/keystone/Account/Login");
            return View();
        }
    }
}