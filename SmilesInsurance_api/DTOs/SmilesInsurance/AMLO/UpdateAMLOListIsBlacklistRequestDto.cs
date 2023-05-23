using System;
using System.ComponentModel.DataAnnotations;

namespace SmilesInsurance_api.DTOs.SmilesInsurance.AMLO
{
    public class UpdateAMLOListIsBlacklistRequestDto
    {
        [Required]
        public Guid AMLOListId { get; set; }

        [Required]
        public Boolean IsBlacklist { get; set; }

        [Required]
        public string Remark { get; set; }
    }
}