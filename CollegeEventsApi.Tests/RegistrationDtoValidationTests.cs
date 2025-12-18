using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CollegeEventsApi.Dtos;
using Xunit;

namespace CollegeEventsApi.Tests.Unit;

public sealed class RegistrationApiBehaviorTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RegistrationApiBehaviorTests(CustomWebApplicationFactory factory)
        => _client = factory.CreateClient(new() { BaseAddress = new Uri("http://localhost") });

    [Fact]
    public async Task Register_EventId_Zero_Returns_404_EventNotFound()
    {
        var dto = new RegistrationCreateDto { EventId = 0 };

        var res = await _client.PostAsJsonAsync("/api/Registrations", dto);

     
        Assert.Equal(HttpStatusCode.NotFound, res.StatusCode);

        var body = await res.Content.ReadAsStringAsync();
        Assert.Contains("Event not found", body, StringComparison.OrdinalIgnoreCase);
    }
}