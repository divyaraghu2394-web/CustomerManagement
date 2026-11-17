
namespace OrderManagement.API.Services;

public class CustomerService
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CustomerService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<string> GetCustomersAsync()
    {
        var client = _httpClientFactory.CreateClient("CustomerApi");

        var response = await client.GetAsync("/api/v1/customer/");

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
}
