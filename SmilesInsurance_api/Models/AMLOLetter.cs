﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmilesInsurance_api.Models
{
    [Table("AMLOLetter", Schema = "amlo")]
    public partial class AMLOLetter
    {
        public AMLOLetter()
        {
            AMLOLists = new HashSet<AMLOList>();
        }

        [Key]
        public Guid AMLOLetterId { get; set; }
        [StringLength(100)]
        public string AMLOLetterName { get; set; }
        public bool IsActive { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public int? UpdateByUserId { get; set; }

        [InverseProperty(nameof(AMLOList.AMLOLetter))]
        public virtual ICollection<AMLOList> AMLOLists { get; set; }
    }
}