using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace englishProject.Infrastructure.ViewModel
{
    public class UserPasswordViewModel
    {
        [MaxLength(8, ErrorMessage = "En fazla 8 karakterli olmalı")]
        [MinLength(6, ErrorMessage = "En az 6 karakter olmalı")]
        [JsonProperty("currentPassword")]
        [Display(Prompt = "Şifreniz"), DataType(DataType.Password)]
        [Required(ErrorMessage = "Şifre alanı gerekli")]
        public string CurrentPassword { get; set; }

        [MaxLength(8, ErrorMessage = "En fazla 8 karakterli olmalı")]
        [MinLength(6, ErrorMessage = "En az 6 karakter olmalı")]
        [JsonProperty("newPassword")]
        [Display(Prompt = "Şifreniz"), DataType(DataType.Password)]
        [Required(ErrorMessage = "Şifre alanı gerekli")]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "Şifreleriniz uyuşmuyor")]
        [Required(ErrorMessage = "Şifre alanı gerekli")]
        [Display(Prompt = "Şifreniz tekrar"), DataType(DataType.Password)]
        [MaxLength(8, ErrorMessage = "En fazla 8 karakterli olmalı")]
        [MinLength(6, ErrorMessage = "En az 6 karakter olmalı")]
        [JsonProperty("confirmPassword")]
        public string ConfirmPassowrd { get; set; }
    }
}