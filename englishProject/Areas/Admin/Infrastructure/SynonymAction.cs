using englishProject.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace englishProject.Areas.Admin.Infrastructure
{
    public class SynonymAction : ISynonym
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private EnglishProjectDBEntities entities;

        public SynonymAction()
        {
            entities = new EnglishProjectDBEntities();
        }

        public bool AddWord(SynonymWord word)
        {
            if (entities.SynonymWord.Any(a => a.synonymTurkish.Trim() == word.synonymTurkish.Trim()))
            {
                return false;
            }
            else
            {
                entities.Entry(word).State = EntityState.Added;
                entities.SaveChanges();
                return true;
            }
        }

        public IEnumerable<SynonymWord> SynonymWords(int levelId)
        {
            try
            {
                return entities.SynonymWord.Where(a => a.levelId == levelId).OrderByDescending(a => a.synonymId).ToList();
            }
            catch (Exception)
            {
                return new List<SynonymWord>();
            }
        }

        public bool UpdateSynonymWord(SynonymWord word)
        {
            entities.Entry(word).State = EntityState.Modified;
            entities.SaveChanges();
            return true;
        }

        public bool DeleteSynonymWord(int synonymId)
        {
            SynonymWord w = entities.SynonymWord.First(a => a.synonymId == synonymId);

            entities.SynonymWord.Remove(w);

            entities.SaveChanges();
            return true;
        }

        public SynonymWord GetSynonymWord(int synonymId)
        {
            return entities.SynonymWord.Find(synonymId);
        }
    }
}