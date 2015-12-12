using englishProject.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace englishProject.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        // GET: Admin/Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SpeedWord(int subLevel = 1, int levelNumber = 1, int kind = 1)
        {
            Operations operations = new Operations();

            return View(operations.GetExam((SubLevel)subLevel, levelNumber, kind));
        }
    }
}