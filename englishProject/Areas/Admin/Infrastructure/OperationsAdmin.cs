using englishProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace englishProject.Areas.Admin.Infrastructure
{
    public static class OperationsAdmin
    {
        private static readonly EnglishProjectDBEntities entities = new EnglishProjectDBEntities();

        public static List<log> GetLogs()
        {
            return entities.log.OrderByDescending(a => a.Date).Take(30).ToList();
        }
    }
}