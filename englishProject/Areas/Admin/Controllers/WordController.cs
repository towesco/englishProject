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

        public ActionResult Words(int selectLevel = 1, int selectKind = 1)
        {
            ViewBag.selectLevel = new SelectList(HelperMethod.GetLevelSelectListItems(), "Value", "Text");
            ViewBag.selectKind = new SelectList(HelperMethod.getListKind(), "Value", "Text");
            return View(entities.Words(selectLevel, selectKind));
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
            ViewBag.kind = new SelectList(HelperMethod.getListKind(w.kind), "Value", "Text", w.kind);
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