using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace englishProject.Infrastructure.ViewModel
{
    public class ContactVM
    {
        [Required(ErrorMessage = "İsim alanını boş bırakılamaz")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email adresi gerekli")]
        [EmailAddress(ErrorMessage = "geçersiz email")]
        public string Email { get; set; }

        public string Phone { get; set; }

        [Required(ErrorMessage = "Lütfen mesajınızı yazınız")]
        public string Message { get; set; }
    }
}