using System;

namespace SmilesInsurance_api.DTOs.SmilesInsurance.AMLO
{
    public class GetAMLOListRequestDto : PaginationDto
    {
        public string SearchText { get; set; }
        public Guid? AMLOLetterId { get; set; }

        public string OrderingField { get; set; }

        public bool AscendingOrder { get; set; } = true;
    }
}