using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace englishProject.Areas.Admin.Infrastructure
{
    public class boxLevelCount
    {
        public string BoxName { get; set; }

        public Dictionary<int, string> Levels { get; set; }
    }
}