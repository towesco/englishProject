using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace englishProject.Infrastructure
{
    public class UserProfilView
    {
        public string picture { get; set; }

        public string userName { get; set; }

        public int TotalPuan { get; set; }

        public List<UserProfilBox> UserProfilBoxs { get; set; }
    }

    public class UserProfilBox
    {
        public string BoxName { get; set; }

        public int LevelCurrent { get; set; }
    }
}