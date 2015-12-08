using englishProject.Areas.Admin.Infrastructure;
using englishProject.Infrastructure;
using englishProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace englishProject.Areas.Admin.Controllers
{
    public class LevelController : Controller
    {
        private ILevel entities;

        public LevelController(ILevel entities)
        {
            this.entities = entities;
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Levels()
        {
            return View(entities.Levels());
        }

        public ActionResult AddLevel()
        {
            ViewBag.list = HelperMethod.getListKind();

            return View(new Level());
        }

        [HttpPost]
        public ActionResult AddLevel(Level level)
        {
            entities.AddLevel(level);
            return RedirectToAction("Levels");
        }

        public ActionResult UpdateLevel(int levelNumber, int kind)
        {
            return View(entities.GetLevel(levelNumber, kind));
        }

        [HttpPost]
        public ActionResult UpdateLevel(Level level)
        {
            entities.UpdateLevel(level);
            return RedirectToAction("Levels");
        }

        [HttpPost]
        public ActionResult DeleteLevel(int levelNumber, int kind)
        {
            entities.DeleteLevel(levelNumber, kind);
            return RedirectToAction("Levels");
        }

        public ActionResult deneme()
        {
            return View();
        }
    }
}