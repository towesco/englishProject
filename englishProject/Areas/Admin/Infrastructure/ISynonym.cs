using englishProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace englishProject.Areas.Admin.Infrastructure
{
    public interface ISynonym
    {
        bool AddWord(SynonymWord word);

        IEnumerable<SynonymWord> SynonymWords(int levelId);

        bool UpdateSynonymWord(SynonymWord word);

        bool DeleteSynonymWord(int synonymId);

        SynonymWord GetSynonymWord(int synonymId);
    }
}