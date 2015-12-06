using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace englishProject.Infrastructure.ViewModel
{
    public class UserSignUpVM
    {
        [JsonProperty("email")]
        [Display(Prompt = "Email adresiniz"), DataType(DataType.EmailAddress, ErrorMessage = "Email adresiniz geçersiz")]
        [Required(ErrorMessage = "Email alanı gerekli")]
        [EmailAddress(ErrorMessage = "Email adresi geçersiz")]
        public string Email { get; set; }

        [MaxLength(8, ErrorMessage = "En fazla 8 karakterli olmalı")]
        [MinLength(6, ErrorMessage = "En az 6 karakter olmalı")]
        [JsonProperty("password")]
        [Display(Prompt = "Şifreniz"), DataType(DataType.Password)]
        [Required(ErrorMessage = "Şifre alanı gerekli")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Şifreleriniz uyuşmuyor")]
        [Required(ErrorMessage = "Şifre alanı gerekli")]
        [Display(Prompt = "Şifreniz tekrar"), DataType(DataType.Password)]
        [MaxLength(8, ErrorMessage = "En fazla 8 karakterli olmalı")]
        [MinLength(6, ErrorMessage = "En az 6 karakter olmalı")]
        [JsonProperty("confirmPassword")]
        public string ConfirmPassowrd { get; set; }
    }
}