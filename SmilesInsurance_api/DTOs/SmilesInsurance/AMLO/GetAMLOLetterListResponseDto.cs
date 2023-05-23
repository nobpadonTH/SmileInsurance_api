using System;

namespace SmilesInsurance_api.DTOs.SmilesInsurance.AMLO
{
    public class GetAMLOLetterListResponseDto
    {
        public Guid AMLOLetterId { get; set; }
        public string AMLOLetterName { get; set; }
    }
}