using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CollegeEventsApi.Dtos;
using Xunit;

namespace CollegeEventsApi.Tests;

public sealed class RegistrationsTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RegistrationsTests(CustomWebApplicationFactory factory)
        => _client = factory.CreateClient(new() { BaseAddress = new Uri("http://localhost") });

    [Fact]
    public async Task Can_Register_For_Event_And_Read_Back()
    {
      
        var eventsRes = await _client.GetAsync("/api/Events");
        eventsRes.StatusCode.ShouldBe(HttpStatusCode.OK);

        var events = await eventsRes.Content.ReadFromJsonAsync<List<EventReadDto>>();
        Assert.NotNull(events);
        Assert.NotEmpty(events);

        var eventId = events!.First().Id;

       
        var reg = new RegistrationCreateDto { EventId = eventId };

        var regRes = await _client.PostAsJsonAsync("/api/Registrations", reg);
        Assert.True(regRes.StatusCode is HttpStatusCode.OK or HttpStatusCode.Created);

     
        var myRes = await _client.GetAsync("/api/Registrations/my");
        myRes.StatusCode.ShouldBe(HttpStatusCode.OK);

        var mine = await myRes.Content.ReadFromJsonAsync<List<RegistrationReadDto>>();
        Assert.NotNull(mine);

        Assert.Contains(mine!, r => r.EventId == eventId);
    }
}