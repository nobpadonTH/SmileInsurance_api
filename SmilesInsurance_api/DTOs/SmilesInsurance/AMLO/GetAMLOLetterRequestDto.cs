using SmilesInsurance_api.DTOs;

namespace SmilesInsurance_api.Services.SmilesInsurance.AMLO
{
    public class GetAMLOLetterRequestDto : PaginationDto
    {
        public string AMLOLetterName { get; set; }
        public string OrderingField { get; set; }

        public bool AscendingOrder { get; set; } = true;
    }
}