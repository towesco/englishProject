using englishProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace englishProject.Infrastructure
{
    public class Operations
    {
        private static readonly EnglishProjectDBEntities entities;

        static Operations()
        {
            entities = new EnglishProjectDBEntities();
        }

        public static List<Box> GetBoxs(Kind kind)
        {
            int _kind = (int)kind;

            return entities.Box.Include("Level").Where(a => a.kind == _kind).OrderBy(a => a.boxNumber).ToList();
        }
    }
}