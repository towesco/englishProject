using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace englishProject.Infrastructure.ViewModel
{
    public class UserSignInVM
    {
        [JsonProperty("email")]
        [Display(Prompt = "Email adresiniz"), DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Email alanı gerekli")]
        [EmailAddress(ErrorMessage = "Email adresi geçersiz")]
        public string Email { get; set; }

        [MaxLength(8, ErrorMessage = "Enfazla 8 karakterli olmalı")]
        [MinLength(6, ErrorMessage = "En az 6 karakter olmalı")]
        [JsonProperty("password")]
        [Display(Prompt = "Şifreniz"), DataType(DataType.Password)]
        [Required(ErrorMessage = "şifre alanı gerekli")]
        public string Password { get; set; }

        public bool MeRemember { get; set; }
    }
}