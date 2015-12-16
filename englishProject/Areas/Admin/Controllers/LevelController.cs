using englishProject.Areas.Admin.Infrastructure;
using englishProject.Infrastructure;
using englishProject.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace englishProject.Areas.Admin.Controllers
{
    public class LevelController : Controller
    {
        private readonly ILevel entities;

        private readonly HelperMethod helperMethod;

        public LevelController(ILevel entities)
        {
            this.entities = entities;
            helperMethod = new HelperMethod();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Levels(int boxNumber = 1, int selectKind = 1)
        {
            ViewBag.selectKind = new SelectList(helperMethod.getListKind(), "Value", "Text");
            ViewBag.boxNumber = new SelectList(helperMethod.GetBoxSelectListItems(), "Value", "Text");

            return View(entities.Levels(boxNumber, (Kind)selectKind));
        }

        public ActionResult AddLevel()
        {
            ViewBag.boxNumber = new SelectList(helperMethod.GetBoxSelectListItems(), "Value", "Text");
            ViewBag.kind = helperMethod.getListKind();
            ViewBag.levelNumberAppear = helperMethod.GetLevelNumberAppearListItems();
            ViewBag.levelModul = helperMethod.GetModulListItems();
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

            ViewBag.kind = new SelectList(helperMethod.getListKind(), "Value", "Text", level.kind.ToString(CultureInfo.InvariantCulture));
            ViewBag.boxNumber = new SelectList(helperMethod.GetBoxSelectListItems(), "Value", "Text", level.boxNumber.ToString(CultureInfo.InvariantCulture));

            ViewBag.levelModul = new SelectList(helperMethod.GetModulListItems(), "Value", "Text", level.levelModul.ToString(CultureInfo.InvariantCulture));

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