using SmilesInsurance_api.Models;
using System;
using System.Collections.Generic;

namespace SmilesInsurance_api.DTOs.SmilesInsurance.AMLO
{
    public class GetAMLOLetterResponseDto
    {
        public Guid AMLOLetterId { get; set; }
        public string AMLOLetterName { get; set; }
        public int? CreatedByUserId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? AMLOListCount { get; set; }
        public AMLOList AMLOList { get; set; }
        public List<GetAMLOListResponseDto> AMLOLists { get; set; }
    }
}