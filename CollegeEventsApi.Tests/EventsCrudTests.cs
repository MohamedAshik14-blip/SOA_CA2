using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CollegeEventsApi.Dtos;
using Xunit;

namespace CollegeEventsApi.Tests;

public sealed class EventsCrudTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public EventsCrudTests(CustomWebApplicationFactory factory)
        => _client = factory.CreateClient(new() { BaseAddress = new Uri("http://localhost") });

    [Fact]
    public async Task GetAll_Events_ReturnsOk_And_List()
    {
        var res = await _client.GetAsync("/api/Events");
        res.StatusCode.ShouldBe(HttpStatusCode.OK);

        var data = await res.Content.ReadFromJsonAsync<List<EventReadDto>>();
        Assert.NotNull(data);

    
    }

    [Fact]
    public async Task Create_Update_Delete_Event_Works()
    {
        var now = DateTime.UtcNow;

   
        var create = new EventCreateDto
        {
            Title = "Test Create",
            Description = "Created from tests",
            StartTime = now.AddDays(10),
            EndTime = now.AddDays(10).AddHours(2),
            Capacity = 25,
            CategoryId = 1,
            VenueId = 1
        };

        var createRes = await _client.PostAsJsonAsync("/api/Events", create);
        createRes.StatusCode.ShouldBeOkOrCreated();

        var created = await createRes.Content.ReadFromJsonAsync<EventReadDto>();
        Assert.NotNull(created);
        Assert.True(created.Id > 0);

    
        var update = new EventUpdateDto
        {
            Title = "Test Updated",
            Description = "Updated from tests",
            StartTime = create.StartTime,
            EndTime = create.EndTime,
            Capacity = 30,
            CategoryId = 1,
            VenueId = 1
        };

        var updateRes = await _client.PutAsJsonAsync($"/api/Events/{created.Id}", update);
        updateRes.StatusCode.ShouldBeOkOrNoContent();

        
        var getRes = await _client.GetAsync($"/api/Events/{created.Id}");
        getRes.StatusCode.ShouldBe(HttpStatusCode.OK);

        var got = await getRes.Content.ReadFromJsonAsync<EventReadDto>();
        Assert.NotNull(got);
        Assert.Equal("Test Updated", got.Title);

       
        var delRes = await _client.DeleteAsync($"/api/Events/{created.Id}");
        Assert.True(delRes.StatusCode is HttpStatusCode.NoContent or HttpStatusCode.OK);
    }
}