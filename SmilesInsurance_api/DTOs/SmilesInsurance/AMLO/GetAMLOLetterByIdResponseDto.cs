using System.ComponentModel.DataAnnotations;
using System;

namespace SmilesInsurance_api.DTOs.SmilesInsurance.AMLO
{
    public class GetAMLOLetterByIdResponseDto
    {
        public Guid AMLOLetterId { get; set; }
        public string AMLOLetterName { get; set; }
        public bool IsActive { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}