using System;
using System.ComponentModel.DataAnnotations;

namespace SmilesInsurance_api.DTOs.SmilesInsurance.AMLO
{
    public class InsertAMLOListRequestDto
    {
        public Guid AMLOLetterId { get; set; }

        [StringLength(20)]
        public string IdCardNo { get; set; }

        [StringLength(100)]
        public string PassportNo { get; set; }

        public DateTime? BirthDate { get; set; }

        [StringLength(100)]
        public string FirstName { get; set; }

        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(100)]
        public string FirstName_eng { get; set; }

        [StringLength(100)]
        public string LastName_eng { get; set; }
    }
}