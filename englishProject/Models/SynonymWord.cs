//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace englishProject.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class SynonymWord
    {
        public int synonymId { get; set; }
        public int levelId { get; set; }
        public string synonymTurkish { get; set; }
        public string synonym1 { get; set; }
        public string synonym2 { get; set; }
        public string synonym3 { get; set; }
    
        public virtual Level Level { get; set; }
    }
}
