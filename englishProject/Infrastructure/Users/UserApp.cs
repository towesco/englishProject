using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace englishProject.Infrastructure.Users
{
    public class UserApp : IdentityUser
    {
        [Column(TypeName = "NVARCHAR")]
        [StringLength(200)]
        public string PicturePath { get; set; }

        public DateTime? BirthDay { get; set; }

        public bool? Gender { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [StringLength(50)]
        public string Name { get; set; }

        [Column(TypeName = "NVARCHAR")]
        [StringLength(50)]
        public string SurName { get; set; }

        public int? City { get; set; }

        public DateTime createTime { get; set; }
    }
}