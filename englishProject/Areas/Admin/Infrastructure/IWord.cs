using englishProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace englishProject.Areas.Admin.Infrastructure
{
    public interface IWord
    {
        bool AddWord(Word word);

        IEnumerable<Word> Words(int levelNumber, int kind, int boxNumber);

        bool UpdateWord(Word word);

        bool DeleteWord(int wordId);

        Word GetWord(int wordId);

        IEnumerable<Level> GetLevels();
    }
}