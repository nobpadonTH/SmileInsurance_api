using System.ComponentModel.DataAnnotations;

namespace SmilesInsurance_api.DTOs.SmilesInsurance.AMLO
{
    public class InsertAMLOLetterRequestDto
    {
        [StringLength(100)]
        [Required]
        public string AMLOLetterName { get; set; }
    }
}