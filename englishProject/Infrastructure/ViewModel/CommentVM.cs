using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace englishProject.Infrastructure.ViewModel
{
    public class CommentIssueVM
    {
        [Required(ErrorMessage = "Bu alanı doldurunuz.")]
        public string CommentIssue { get; set; }

        public int ExceptId { get; set; }

        public int? ReplyId { get; set; }

        //yorum ne için kaydedildiğini öğrenmek için levellar için 1 olarak belirledik
        public int CommentKind { get; set; }

        //1 olursa yorum 2 olursa problem
        public int Kind { get; set; }

        public string UserId { get; set; }
    }
}