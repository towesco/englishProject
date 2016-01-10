using englishProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace englishProject.Infrastructure.ViewModel
{
    public class UserProfilView
    {
        public string picture { get; set; }

        public string userName { get; set; }

        public int TotalPuan { get; set; }

        public List<userProggress_Result> UserProfilBoxs { get; set; }
    }
}