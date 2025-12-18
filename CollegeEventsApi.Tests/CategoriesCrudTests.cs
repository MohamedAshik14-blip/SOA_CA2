using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using CollegeEventsApi.Dtos;
using Xunit;

namespace CollegeEventsApi.Tests;

public sealed class CategoriesCrudTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public CategoriesCrudTests(CustomWebApplicationFactory factory)
        => _client = factory.CreateClient(new() { BaseAddress = new Uri("http://localhost") });

    [Fact]
    public async Task Categories_CRUD_Works()
    {
       
        var listRes = await _client.GetAsync("/api/Categories");
        listRes.StatusCode.ShouldBe(HttpStatusCode.OK);

        
        var createRes = await _client.PostAsJsonAsync(
            "/api/Categories",
            new CategoryCreateDto { Name = "TestCat" });

        createRes.StatusCode.ShouldBeOkCreatedOrNoContent();

        CategoryReadDto? created = null;

      
        if (createRes.StatusCode != HttpStatusCode.NoContent)
        {
            created = await createRes.Content.ReadFromJsonAsync<CategoryReadDto>();
        }

    
        if (created is null)
        {
            var listAfterCreate = await _client.GetFromJsonAsync<CategoryReadDto[]>("/api/Categories");
            Assert.NotNull(listAfterCreate);
            created = Array.Find(listAfterCreate, c => c.Name == "TestCat");
        }

        Assert.NotNull(created);
        Assert.True(created!.Id > 0);

      
        var updateRes = await _client.PutAsJsonAsync(
            $"/api/Categories/{created.Id}",
            new CategoryUpdateDto { Name = "TestCatUpdated" });

        updateRes.StatusCode.ShouldBeOkOrNoContent();

    
        var delRes = await _client.DeleteAsync($"/api/Categories/{created.Id}");
        delRes.StatusCode.ShouldBe(HttpStatusCode.NoContent);
    }
}