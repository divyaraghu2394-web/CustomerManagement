using System;
using System.Collections.Generic;
using System.Text;

namespace CustomerManagement.Core.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }

        public string CustomerName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Mobile { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;
    }
}
