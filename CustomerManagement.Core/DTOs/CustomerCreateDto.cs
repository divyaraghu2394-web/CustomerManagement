namespace CustomerManagement.Core.DTOs
{
    public class CustomerCreateDto
    {
        public string CustomerName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
    }
}