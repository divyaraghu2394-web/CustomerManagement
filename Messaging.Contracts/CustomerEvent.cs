
namespace Messaging.Contracts;

public class CustomerEvent
{
    public int CustomerId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string EventType { get; set; } = string.Empty;
}
