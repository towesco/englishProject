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

        public static List<SelectListItem> getListKind(int value = 0)
        {
            return (from object item in Enum.GetValues(typeof(Kind))
                    select new System.Web.Mvc.SelectListItem()
                    {
                        Text = item.ToString(),
                        Value = ((int)item).ToString(),
                        Selected = ((int)item) == value ? true : false
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
                                 Text = a.levelName,
                                 Value = a.levelNumber.ToString(),
                                 Selected = false
                             })
                     .ToList();
        }
    }
}