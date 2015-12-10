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
    public class WordController : Controller
    {
        private readonly IWord entities;

        public WordController(IWord word)
        {
            entities = word;
        }

        public ActionResult Words(int levelNumber = 1, int kind = 1, int boxNumber = 1)
        {
            ViewBag.boxNumber = new SelectList(HelperMethod.GetBoxSelectListItems(), "Value", "Text");
            ViewBag.levelNumber = new SelectList(HelperMethod.GetLevelSelectListItems(), "Value", "Text");
            ViewBag.kind = new SelectList(HelperMethod.getListKind(), "Value", "Text");
            return View(entities.Words(levelNumber, kind, boxNumber));
        }

        public ActionResult AddWord()
        {
            ViewBag.kind = HelperMethod.getListKind();
            return View(new Word());
        }

        public ActionResult UpdateWord(int wordId)
        {
            var w = entities.GetWord(wordId);

            ViewBag.levelNumber = new SelectList(HelperMethod.GetLevelSelectListItems(), "Value", "Text", w.levelNumber);
            ViewBag.kind = new SelectList(HelperMethod.getListKind(), "Value", "Text", w.kind);
            return View(w);
        }

        [HttpPost]
        public ActionResult UpdateWord(Word w)
        {
            entities.UpdateWord(w);

            return RedirectToAction("Words", new { selectLevel = w.levelNumber, selectKind = w.kind });
        }

        [HttpPost]
        public ActionResult DeleteWord2(int wordId)
        {
            entities.DeleteWord(wordId);
            return RedirectToAction("Words");
        }

        public JsonResult CreateWord(Word word)
        {
            entities.AddWord(word);

            return Json(word.wordId, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteWord(int wordId)
        {
            return Json(entities.DeleteWord(wordId), JsonRequestBehavior.AllowGet);
        }
    }
}