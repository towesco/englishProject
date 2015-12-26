using englishProject.Infrastructure;
using englishProject.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Management;
using System.Web.Mvc;
using System.Web.WebPages.Html;
using SelectListItem = System.Web.Mvc.SelectListItem;

namespace englishProject.Infrastructure
{
    public class HelperMethod
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private EnglishProjectDBEntities entities;

        public HelperMethod()
        {
            entities = new EnglishProjectDBEntities();
        }

        /// <summary>
        /// Genel Dil seçeneklerini getirir İngilizce=1,Almanca 2
        /// </summary>
        /// <returns></returns>

        /// <summary>
        /// Level tablosundan  levelName ve levelın ait olduğu boxName çeker
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetLevelSelectListItems()
        {
            return
                 entities.Level.OrderBy(a => a.levelId).ToList()
                     .Select(
                         a =>
                             new SelectListItem
                             {
                                 Text = string.Format("{0}({2})---->{1} kutusu", a.levelName, a.Box.boxName, Enum.GetName(typeof(Modul), a.levelModul) ?? ""),
                                 Value = a.levelId.ToString(CultureInfo.InvariantCulture),
                                 Selected = false
                             })
                     .ToList();
        }

        /// <summary>
        /// Box tablosundan boxName ve BoxNumber çeker
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetBoxSelectListItems()
        {
            return entities.Box.ToList().Select(a => new SelectListItem
            {
                Text = a.boxName,
                Value = a.boxId.ToString(CultureInfo.InvariantCulture),
                Selected = false
            }).ToList();
        }

        /// <summary>
        /// Level tablosundaki LevelNumberAppear değeri için 1 den 50 kadar sayı çeker
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetLevelNumberListItems()
        {
            return
                Enumerable.Range(1, 50)
                    .Select(a => new SelectListItem { Text = a.ToString(CultureInfo.InvariantCulture), Value = a.ToString(CultureInfo.InvariantCulture), Selected = false })
                    .ToList();
        }

        public List<SelectListItem> GetModulListItems()
        {
            return
                Enumerable.Range(1, 10)
                    .Select(a => new SelectListItem { Text = string.Format("{0}-{1}", a.ToString(CultureInfo.InvariantCulture), Enum.GetName(typeof(Modul), a) ?? "boş"), Value = a.ToString(CultureInfo.InvariantCulture), Selected = false })
                    .ToList();
        }
    }
}