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
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private wordboxe_englishEntities entities;

        public LevelAction()
        {
            entities = new wordboxe_englishEntities();
        }

        public IEnumerable<Level> Levels(int boxId)
        {
            return entities.Box.Find(boxId).Level.ToList();
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

        public bool DeleteLevel(int levelId)
        {
            Level level = entities.Level.FirstOrDefault(a => a.levelId == levelId);
            if (level != null)
            {
                entities.Entry(level).State = EntityState.Deleted;
                entities.SaveChanges();
            }

            return true;
        }

        public Level GetLevel(int levelId)
        {
            return entities.Level.First(a => a.levelId == levelId);
        }
    }
}