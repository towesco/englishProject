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
    public class SynonymWordController : Controller
    {
        private ISynonym entities;

        private readonly HelperMethod helperMethod;

        public SynonymWordController(ISynonym synonym)
        {
            entities = synonym;
            helperMethod = new HelperMethod();
        }

        public ActionResult SynonymWords(int levelId = 1)
        {
            ViewBag.levelId = new SelectList(helperMethod.GetLevelSelectListItems(Modul.SynonymWordModul), "Value", "Text");

            return View(entities.SynonymWords(levelId));
        }

        public ActionResult AddSynonymWord()
        {
            return View(new SynonymWord());
        }

        public ActionResult UpdateSynonymWord(int synonymId)
        {
            var w = entities.GetSynonymWord(synonymId);

            ViewBag.levelId = new SelectList(helperMethod.GetLevelSelectListItems(Modul.SynonymWordModul), "Value", "Text", w.levelId);

            return View(w);
        }

        [HttpPost]
        public ActionResult UpdateSynonymWord(SynonymWord w)
        {
            entities.UpdateSynonymWord(w);

            return RedirectToAction("SynonymWords", new { w.levelId });
        }

        [HttpPost]
        public ActionResult DeleteSynonymWord2(int synonymId)
        {
            SynonymWord w = entities.GetSynonymWord(synonymId);
            entities.DeleteSynonymWord(synonymId);
            return RedirectToAction("SynonymWords", new { w.levelId });
        }

        public JsonResult CreateSynonymWord(SynonymWord synonymWord)
        {
            string json = string.Format("{0}-{1}", entities.AddWord(synonymWord), synonymWord.synonymId);

            return Json(json, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteSynonymWord(int synonymId)
        {
            return Json(entities.DeleteSynonymWord(synonymId), JsonRequestBehavior.AllowGet);
        }

        // GET: Admin/SynonymWord
        public ActionResult Index()
        {
            return View();
        }
    }
}