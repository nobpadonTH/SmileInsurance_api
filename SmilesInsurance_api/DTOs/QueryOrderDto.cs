namespace SmilesInsurance_api.DTOs
{
    public class QuerySortDto
    {
        public string OrderingField { get; set; } = null;
        public bool AscendingOrder { get; set; } = true;
    }
}