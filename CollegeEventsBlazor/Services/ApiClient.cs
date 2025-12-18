using System.Net.Http.Json;
using CollegeEventsBlazor.Models;

namespace CollegeEventsBlazor.Services;

public class ApiClient
{
    private readonly IHttpClientFactory _factory;
    public ApiClient(IHttpClientFactory factory) => _factory = factory;

    private HttpClient Api => _factory.CreateClient("Api");



    public async Task<List<EventDto>> GetEventsAsync() =>
        await Api.GetFromJsonAsync<List<EventDto>>("api/Events") ?? new();

    public async Task CreateEventAsync(EventDto dto)
    {
       
        var payload = new
        {
            title = dto.Title,
            description = dto.Description,
            startTime = dto.StartTime,
            endTime = dto.EndTime,
            capacity = dto.Capacity,
            venueId = dto.VenueId,
            categoryId = dto.CategoryId
        };

        var res = await Api.PostAsJsonAsync("api/Events", payload);
        if (!res.IsSuccessStatusCode) throw new Exception(await res.Content.ReadAsStringAsync());
    }

    public async Task UpdateEventAsync(int id, EventDto dto)
    {
        var payload = new
        {
            title = dto.Title,
            description = dto.Description,
            startTime = dto.StartTime,
            endTime = dto.EndTime,
            capacity = dto.Capacity,
            venueId = dto.VenueId,
            categoryId = dto.CategoryId
        };

        var res = await Api.PutAsJsonAsync($"api/Events/{id}", payload);
        if (!res.IsSuccessStatusCode) throw new Exception(await res.Content.ReadAsStringAsync());
    }

    public async Task DeleteEventAsync(int id)
    {
        var res = await Api.DeleteAsync($"api/Events/{id}");
        if (!res.IsSuccessStatusCode) throw new Exception(await res.Content.ReadAsStringAsync());
    }

    public async Task<List<VenueDto>> GetVenuesAsync() =>
        await Api.GetFromJsonAsync<List<VenueDto>>("api/Venues") ?? new();

    public async Task<List<CategoryDto>> GetCategoriesAsync() =>
        await Api.GetFromJsonAsync<List<CategoryDto>>("api/Categories") ?? new();

 

    public async Task RegisterForEventAsync(int eventId)
    {
        var res = await Api.PostAsJsonAsync("api/Registrations", new RegistrationCreateDto { EventId = eventId });
        if (!res.IsSuccessStatusCode) throw new Exception(await res.Content.ReadAsStringAsync());
    }

    public async Task<List<RegistrationReadDto>> MyRegistrationsAsync() =>
        await Api.GetFromJsonAsync<List<RegistrationReadDto>>("api/Registrations/my") ?? new();

    public async Task CheckInAsync(int eventId)
    {
        var res = await Api.PostAsync($"api/Registrations/{eventId}/checkin", null);
        if (!res.IsSuccessStatusCode) throw new Exception(await res.Content.ReadAsStringAsync());
    }

    public async Task CheckOutAsync(int eventId)
    {
        var res = await Api.PostAsync($"api/Registrations/{eventId}/checkout", null);
        if (!res.IsSuccessStatusCode) throw new Exception(await res.Content.ReadAsStringAsync());
    }

    public async Task WithdrawAsync(int eventId)
    {
        var res = await Api.DeleteAsync($"api/Registrations/{eventId}");
        if (!res.IsSuccessStatusCode) throw new Exception(await res.Content.ReadAsStringAsync());
    }

  

    public async Task<List<RegistrationReadDto>> AllRegistrationsAsync() =>
        await Api.GetFromJsonAsync<List<RegistrationReadDto>>("api/Registrations") ?? new();

    public async Task AdminRemoveAsync(int studentId, int eventId)
    {
        var res = await Api.DeleteAsync($"api/Registrations/admin/remove?studentId={studentId}&eventId={eventId}");
        if (!res.IsSuccessStatusCode) throw new Exception(await res.Content.ReadAsStringAsync());
    }

    public async Task AdminForceCheckInAsync(int studentId, int eventId)
    {
        var res = await Api.PostAsync($"api/Registrations/admin/checkin?studentId={studentId}&eventId={eventId}", null);
        if (!res.IsSuccessStatusCode) throw new Exception(await res.Content.ReadAsStringAsync());
    }

    public async Task AdminForceCheckOutAsync(int studentId, int eventId)
    {
        var res = await Api.PostAsync($"api/Registrations/admin/checkout?studentId={studentId}&eventId={eventId}", null);
        if (!res.IsSuccessStatusCode) throw new Exception(await res.Content.ReadAsStringAsync());
    }


    public async Task CreateVenueAsync(VenueDto dto)
    {
        var payload = new { name = dto.Name, address = dto.Address };
        var res = await Api.PostAsJsonAsync("api/Venues", payload);
        if (!res.IsSuccessStatusCode) throw new Exception(await res.Content.ReadAsStringAsync());
    }

    public async Task UpdateVenueAsync(int id, VenueDto dto)
    {
        var payload = new { name = dto.Name, address = dto.Address };
        var res = await Api.PutAsJsonAsync($"api/Venues/{id}", payload);
        if (!res.IsSuccessStatusCode) throw new Exception(await res.Content.ReadAsStringAsync());
    }

    public async Task DeleteVenueAsync(int id)
    {
        var res = await Api.DeleteAsync($"api/Venues/{id}");
        if (!res.IsSuccessStatusCode) throw new Exception(await res.Content.ReadAsStringAsync());
    }



    public async Task CreateCategoryAsync(CategoryDto dto)
    {
        var payload = new { name = dto.Name };
        var res = await Api.PostAsJsonAsync("api/Categories", payload);
        if (!res.IsSuccessStatusCode) throw new Exception(await res.Content.ReadAsStringAsync());
    }

    public async Task UpdateCategoryAsync(int id, CategoryDto dto)
    {
        var payload = new { name = dto.Name };
        var res = await Api.PutAsJsonAsync($"api/Categories/{id}", payload);
        if (!res.IsSuccessStatusCode) throw new Exception(await res.Content.ReadAsStringAsync());
    }

    public async Task DeleteCategoryAsync(int id)
    {
        var res = await Api.DeleteAsync($"api/Categories/{id}");
        if (!res.IsSuccessStatusCode) throw new Exception(await res.Content.ReadAsStringAsync());
    }
    
}
