using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Kimyon.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Uygulamanın Açıklama Bölümü";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "İletişim Bölümü";

            return View();
        }
    }
}