using englishProject.Infrastructure;
using englishProject.Models;
using System;
using System.Collections.Generic;
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
        private static readonly EnglishProjectDBEntities entities = new EnglishProjectDBEntities();

        public static List<SelectListItem> getListKind()
        {
            return (from object item in Enum.GetValues(typeof(Kind))
                    select new System.Web.Mvc.SelectListItem()
                    {
                        Text = item.ToString(),
                        Value = ((int)item).ToString(),
                        Selected = false
                    }).ToList();
        }

        public static List<SelectListItem> GetLevelSelectListItems()
        {
            return
                 entities.Level.ToList()
                     .Select(
                         a =>
                             new SelectListItem()
                             {
                                 Text = string.Format("{0}--------------->{1} kutusu", a.levelName, a.Box.boxName),
                                 Value = a.levelNumber.ToString(),
                                 Selected = false
                             })
                     .ToList();
        }

        public static List<SelectListItem> GetBoxSelectListItems()
        {
            return entities.Box.ToList().Select(a => new SelectListItem()
            {
                Text = a.boxName,
                Value = a.boxNumber.ToString(),
                Selected = false
            }).ToList();
        }

        public static List<SelectListItem> GetLevelNumberListItems()
        {
            List<int> ListOne = Enumerable.Range(1, 50).ToList();

            List<int> ListTwo = entities.Level.Select(a => a.levelNumber).ToList();

            List<int> remainingList = ListOne.Except(ListTwo).ToList();

            return
                remainingList.Select(
                    a => new SelectListItem() { Text = a.ToString(), Value = a.ToString(), Selected = false }).ToList();
        }
    }
}