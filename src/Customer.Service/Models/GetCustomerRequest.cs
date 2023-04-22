namespace Customer.Service.Models
{
    public class GetCustomerRequest
    {
        public string SortBy { get; set; }
        public string SortDirection { get; set; }
        public string? SearchText { get; set; }
        public int? StatusFilter { get; set; }

    }
}
