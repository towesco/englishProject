using englishProject.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace englishProject.Areas.Admin.Infrastructure
{
    public class WordAction : IWord
    {
        // ReSharper disable once FieldCanBeMadeReadOnly.Local
        private EnglishProjectDBEntities entities;

        public WordAction()
        {
            entities = new EnglishProjectDBEntities();
        }

        public bool AddWord(Word word)
        {
            entities.Entry(word).State = EntityState.Added;
            entities.SaveChanges();
            return true;
        }

        public IEnumerable<Word> Words(int levelNumber, int kind, int boxNumber)
        {
            try
            {
                return entities.Level.Include("Word")
                          .First(a => a.levelNumber == levelNumber && a.kind == kind && a.boxNumber == boxNumber)
                          .Word.OrderByDescending(a => a.wordId).ToList();
            }
            catch (Exception)
            {
                return new List<Word>();
            }
        }

        public bool UpdateWord(Word word)
        {
            entities.Entry(word).State = EntityState.Modified;
            entities.SaveChanges();
            return true;
        }

        public bool DeleteWord(int wordId)
        {
            entities.Word.Remove(entities.Word.First(a => a.wordId == wordId));
            entities.SaveChanges();
            return true;
        }

        public Word GetWord(int wordId)
        {
            return entities.Word.First(a => a.wordId == wordId);
        }

        public IEnumerable<Level> GetLevels()
        {
            return entities.Level.ToList();
        }
    }
}