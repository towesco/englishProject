using englishProject.Infrastructure;

using englishProject.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace englishProject.Areas.Admin.Infrastructure
{
    public interface ILevel
    {
        IEnumerable<Level> Levels(int boxId);

        bool AddLevel(Level level);

        bool UpdateLevel(Level level);

        bool DeleteLevel(int levelId);

        Level GetLevel(int levelId);
    }
}