using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace englishProject.Infrastructure.ViewModel
{
    public class UserViewModel
    {
        public string Name { get; set; }

        public string SurName { get; set; }

        public string BirthDay { get; set; }

        public bool? Gender { get; set; }

        [Required(ErrorMessage = "Email alanı gerekli")]
        [EmailAddress(ErrorMessage = "Email adresi geçersiz")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Kullanıcı adı gerekli")]
        public string UserName { get; set; }

        public string PhoneNumber { get; set; }

        public int? City { get; set; }

        public string PicturePath { get; set; }

        [MaxLength(8, ErrorMessage = "En fazla 8 karakterli olmalı")]
        [MinLength(6, ErrorMessage = "En az 6 karakter olmalı")]
        public string Password { get; set; }
    }
}