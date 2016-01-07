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
    [Authorize]
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

        public ActionResult Levels(int boxId = 1)
        {
            ViewBag.boxId = new SelectList(helperMethod.GetBoxSelectListItems(), "Value", "Text");

            return View(entities.Levels(boxId));
        }

        public ActionResult AddLevel()
        {
            ViewBag.boxId = new SelectList(helperMethod.GetBoxSelectListItems(), "Value", "Text");

            ViewBag.levelNumber = helperMethod.GetLevelNumberListItems();
            ViewBag.levelModul = helperMethod.GetModulListItems();
            return View(new Level());
        }

        [HttpPost]
        public ActionResult AddLevel(Level level)
        {
            entities.AddLevel(level);
            return RedirectToAction("Levels", new { boxId = level.boxId });
        }

        public ActionResult UpdateLevel(int levelId)
        {
            Level level = entities.GetLevel(levelId);

            ViewBag.boxId = new SelectList(helperMethod.GetBoxSelectListItems(), "Value", "Text", level.boxId.ToString(CultureInfo.InvariantCulture));

            ViewBag.levelModul = new SelectList(helperMethod.GetModulListItems(), "Value", "Text", level.levelModul.ToString(CultureInfo.InvariantCulture));

            return View(level);
        }

        [HttpPost]
        public ActionResult UpdateLevel(Level level)
        {
            entities.UpdateLevel(level);
            return RedirectToAction("Levels", new { level.boxId });
        }

        [HttpPost]
        public ActionResult DeleteLevel(int levelId)
        {
            entities.DeleteLevel(levelId);
            return RedirectToAction("Levels");
        }

        public ActionResult deneme()
        {
            return View();
        }
    }
}