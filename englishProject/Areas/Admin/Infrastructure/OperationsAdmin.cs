using englishProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace englishProject.Areas.Admin.Infrastructure
{
    public static class OperationsAdmin
    {
        private static readonly wordboxe_englishEntities entities = new wordboxe_englishEntities();

        public static List<log> GetLogs()
        {
            return entities.log.OrderByDescending(a => a.Date).Take(30).ToList();
        }
    }
}