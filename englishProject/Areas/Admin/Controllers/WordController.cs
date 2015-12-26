using englishProject.Areas.Admin.Infrastructure;
using englishProject.Infrastructure;
using englishProject.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace englishProject.Areas.Admin.Controllers
{
    public class WordController : Controller
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private IWord entities;

        private HelperMethod helperMethod;

        public WordController(IWord word)
        {
            entities = word;
            helperMethod = new HelperMethod();
        }

        public ActionResult Words(int levelId = 1, int boxId = 1)
        {
            ViewBag.boxId = new SelectList(helperMethod.GetBoxSelectListItems(), "Value", "Text");
            ViewBag.levelId = new SelectList(helperMethod.GetLevelSelectListItems(), "Value", "Text");

            return View(entities.Words(levelId));
        }

        public ActionResult AddWord()
        {
            return View(new Word());
        }

        public ActionResult UpdateWord(int wordId)
        {
            var w = entities.GetWord(wordId);

            ViewBag.levelId = new SelectList(helperMethod.GetLevelSelectListItems(), "Value", "Text", w.levelId);

            return View(w);
        }

        [HttpPost]
        public ActionResult UpdateWord(Word w)
        {
            entities.UpdateWord(w);

            return RedirectToAction("Words", new { selectLevel = w.levelId });
        }

        [HttpPost]
        public ActionResult DeleteWord2(int wordId)
        {
            entities.DeleteWord(wordId);
            return RedirectToAction("Words");
        }

        #region Ajax

        public JsonResult CreateWord(Word word)
        {
            entities.AddWord(word);

            return Json(word.wordId, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeletePicture(string path)
        {
            return Json(entities.DeletePicture(path), JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteWord(int wordId)
        {
            return Json(entities.DeleteWord(wordId), JsonRequestBehavior.AllowGet);
        }

        public ContentResult Upload(HttpPostedFileBase fileUpload)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(fileUpload.FileName);

            fileUpload.SaveAs(Server.MapPath("~/Pictures/WordModul/" + fileName));

            return Content("/Pictures/WordModul/" + fileName);
        }

        #endregion Ajax
    }
}