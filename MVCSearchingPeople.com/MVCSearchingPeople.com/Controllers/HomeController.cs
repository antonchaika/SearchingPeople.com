using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCSearchingPeople.com.Models;

namespace MVCSearchingPeople.com.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Start()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Start(InputData input)
        {
            return View("Result", input);
        }
    }
}
