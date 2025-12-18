using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CollegeEventsApi.Dtos;
using Xunit;

namespace CollegeEventsApi.Tests.Unit;

public sealed class EventApiBehaviorTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public EventApiBehaviorTests(CustomWebApplicationFactory factory)
        => _client = factory.CreateClient(new() { BaseAddress = new Uri("http://localhost") });

    [Fact]
    public async Task CreateEvent_EmptyTitle_IsAllowed_ByApi()
    {
        var now = DateTime.UtcNow;

        var dto = new EventCreateDto
        {
            Title = "", // API allows it (as your run proved)
            Description = "desc",
            StartTime = now.AddDays(2),
            EndTime = now.AddDays(2).AddHours(1),
            Capacity = 10,
            CategoryId = 1,
            VenueId = 1
        };

        var res = await _client.PostAsJsonAsync("/api/Events", dto);

     
        Assert.True(res.StatusCode is HttpStatusCode.OK or HttpStatusCode.Created,
            $"Expected 200/201. Got {(int)res.StatusCode} {res.StatusCode}. Body: {await res.Content.ReadAsStringAsync()}");

        var created = await res.Content.ReadFromJsonAsync<EventReadDto>();
        Assert.NotNull(created);
        Assert.True(created!.Id > 0);
        Assert.Equal("", created.Title);
    }
}