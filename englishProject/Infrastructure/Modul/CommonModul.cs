using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace englishProject.Infrastructure
{
    public class CommonModul<T>
    {
        public int Puan { get; set; }

        public int TotalPuan { get; set; }

        public int Star { get; set; }

        public ModulSubLevel SubLevel { get; set; }

        public List<T> Questions { get; set; }
    }
}