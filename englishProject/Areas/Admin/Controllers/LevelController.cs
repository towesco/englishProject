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

        public ActionResult Levels(int boxNumber = 1, int selectKind = 1)
        {
            ViewBag.selectKind = new SelectList(HelperMethod.getListKind(), "Value", "Text");
            ViewBag.boxNumber = new SelectList(HelperMethod.GetBoxSelectListItems(), "Value", "Text");

            return View(entities.Levels(boxNumber, (Kind)selectKind));
        }

        public ActionResult AddLevel()
        {
            ViewBag.boxNumber = new SelectList(HelperMethod.GetBoxSelectListItems(), "Value", "Text");
            ViewBag.kind = HelperMethod.getListKind();

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
            Level level = entities.GetLevel(levelNumber, kind);

            ViewBag.kind = new SelectList(HelperMethod.getListKind(), "Value", "Text", level.kind.ToString());
            ViewBag.boxNumber = new SelectList(HelperMethod.GetBoxSelectListItems(), "Value", "Text", level.boxNumber.ToString());

            return View(level);
        }

        [HttpPost]
        public ActionResult UpdateLevel(Level level)
        {
            entities.UpdateLevel(level);
            return RedirectToAction("Levels", new { boxNumber = level.boxNumber, kind = level.kind });
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