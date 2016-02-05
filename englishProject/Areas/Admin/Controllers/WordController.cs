using AutoMapper.Internal;
using englishProject.Areas.Admin.Infrastructure;
using englishProject.Infrastructure;
using englishProject.Models;
using System;
using System.IO;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace englishProject.Areas.Admin.Controllers
{
    [Authorize]
    public class WordController : Controller
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private IWord entities;

        private readonly HelperMethod helperMethod;

        public WordController(IWord word)
        {
            entities = word;
            helperMethod = new HelperMethod();
        }

        public ActionResult Words(int levelId = 1)
        {
            ViewBag.boxId = new SelectList(helperMethod.GetBoxSelectListItems(), "Value", "Text");
            ViewBag.levelId = new SelectList(helperMethod.GetLevelSelectListItems(Modul.WordModul), "Value", "Text");

            return View(entities.Words(levelId));
        }

        public ActionResult AddWord(int id)
        {
            ViewBag.id = id;
            return View(new Word());
        }

        public ActionResult UpdateWord(int wordId)
        {
            var w = entities.GetWord(wordId);

            ViewBag.levelId = new SelectList(helperMethod.GetLevelSelectListItems(Modul.WordModul), "Value", "Text", w.levelId);

            return View(w);
        }

        [HttpPost]
        public ActionResult UpdateWord(Word w)
        {
            entities.UpdateWord(w);

            return RedirectToAction("Words", new { levelId = w.levelId });
        }

        [HttpPost]
        public ActionResult DeleteWord2(int wordId)
        {
            Word w = entities.GetWord(wordId);
            entities.DeleteWord(wordId);
            return RedirectToAction("Words", new { levelId = w.levelId });
        }

        #region Ajax

        public JsonResult CreateWord(Word word)
        {
            string json = string.Format("{0}-{1}", entities.AddWord(word), word.wordId);

            return Json(json, JsonRequestBehavior.AllowGet);
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

            WebImage webImage = new WebImage(fileUpload.InputStream);

            webImage.Resize(400, 200, false);

            webImage.Save(Server.MapPath("~/Pictures/WordModul/" + fileName));
            //fileUpload.SaveAs(Server.MapPath("~/Pictures/WordModul/" + fileName));

            return Content("/Pictures/WordModul/" + fileName);
        }

        #endregion Ajax
    }
}