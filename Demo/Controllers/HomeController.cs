using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Demo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return ListView();
        }

        public ActionResult About()
        {
            return ListView();
        }

        public ActionResult ListView()
        {
            var conn = new Models.DemoContext();
            var list = conn.Contacts.Select(d => d);
            return View(list);
        }
    }
}
