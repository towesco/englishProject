using englishProject.Infrastructure;
using englishProject.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace englishProject.Areas.Admin.Infrastructure
{
    public class LevelAction : ILevel
    {
        /// <summary>
        /// Change entities
        /// </summary>
        private readonly EnglishProjectDBEntities entities;

        public LevelAction()
        {
            entities = new EnglishProjectDBEntities();
        }

        public IEnumerable<Level> Levels(int boxNumber, Kind kind)
        {
            return entities.Level.Where(a => a.boxNumber == boxNumber & a.kind == (int)kind).ToList();
        }

        public bool AddLevel(Level level)
        {
            entities.Entry(level).State = EntityState.Added;
            entities.SaveChanges();
            return true;
        }

        public bool UpdateLevel(Level level)
        {
            entities.Entry(level).State = EntityState.Modified;
            entities.SaveChanges();

            return true;
        }

        public bool DeleteLevel(int levelNumber, int kind)
        {
            Level level = entities.Level.FirstOrDefault(a => a.levelNumber == levelNumber && a.kind == kind);
            if (level != null)
            {
                entities.Entry(level).State = EntityState.Deleted;
                entities.SaveChanges();
            }

            return true;
        }

        public Level GetLevel(int levelNumber, int kind)
        {
            return entities.Level.First(a => a.levelNumber == levelNumber && a.kind == kind);
        }
    }
}