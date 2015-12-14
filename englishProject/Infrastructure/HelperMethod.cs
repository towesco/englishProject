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

        public List<SelectListItem> getListKind()
        {
            return (Enum.GetValues(typeof(Kind)).Cast<object>().Select(item => new SelectListItem
            {
                Text = item.ToString(),
                Value = ((int)item).ToString(CultureInfo.InvariantCulture),
                Selected = false
            })).ToList();
        }

        public List<SelectListItem> GetLevelSelectListItems()
        {
            return
                 entities.Level.ToList()
                     .Select(
                         a =>
                             new SelectListItem
                             {
                                 Text = string.Format("{0}--------------->{1} kutusu", a.levelName, a.Box.boxName),
                                 Value = a.levelNumber.ToString(CultureInfo.InvariantCulture),
                                 Selected = false
                             })
                     .ToList();
        }

        public List<SelectListItem> GetBoxSelectListItems()
        {
            return entities.Box.ToList().Select(a => new SelectListItem
            {
                Text = a.boxName,
                Value = a.boxNumber.ToString(CultureInfo.InvariantCulture),
                Selected = false
            }).ToList();
        }

        public List<SelectListItem> GetLevelNumberAppearListItems()
        {
            List<int> ListOne = Enumerable.Range(1, 50).ToList();

            //List<int> ListTwo = entities.Level.Select(a => a.levelNumber).ToList();

            //List<int> remainingList = ListOne.Except(ListTwo).ToList();

            //return
            //    remainingList.Select(
            //        a => new SelectListItem { Text = a.ToString(CultureInfo.InvariantCulture), Value = a.ToString(CultureInfo.InvariantCulture), Selected = false }).ToList();

            return
                Enumerable.Range(1, 50)
                    .Select(a => new SelectListItem() { Text = a.ToString(), Value = a.ToString(), Selected = false })
                    .ToList();
        }
    }
}