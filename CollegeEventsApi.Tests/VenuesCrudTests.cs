using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CollegeEventsApi.Dtos;
using Xunit;

namespace CollegeEventsApi.Tests;

public sealed class VenuesCrudTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public VenuesCrudTests(CustomWebApplicationFactory factory)
        => _client = factory.CreateClient(new() { BaseAddress = new Uri("http://localhost") });

    [Fact]
    public async Task Venues_CRUD_Works()
    {
 
        var listRes = await _client.GetAsync("/api/Venues");
        listRes.StatusCode.ShouldBe(HttpStatusCode.OK);

        var list = await listRes.Content.ReadFromJsonAsync<List<VenueReadDto>>();
        Assert.NotNull(list);

     
        var create = new VenueCreateDto { Name = "TestVenue", Address = "Test Address" };
        var createRes = await _client.PostAsJsonAsync("/api/Venues", create);

       
        createRes.StatusCode.ShouldBeOkOrCreated();

        var created = await createRes.Content.ReadFromJsonAsync<VenueReadDto>();
        Assert.NotNull(created);
        Assert.True(created.Id > 0);

       
        var update = new VenueUpdateDto { Name = "TestVenueUpdated", Address = "Updated Address" };
        var updateRes = await _client.PutAsJsonAsync($"/api/Venues/{created.Id}", update);

       
        updateRes.StatusCode.ShouldBeOkOrNoContent();

       
        var delRes = await _client.DeleteAsync($"/api/Venues/{created.Id}");
        Assert.True(delRes.StatusCode is HttpStatusCode.NoContent or HttpStatusCode.OK);
    }
}