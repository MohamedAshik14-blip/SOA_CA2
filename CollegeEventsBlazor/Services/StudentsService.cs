using System.Net.Http.Json;
using CollegeEventsBlazor.Models;

namespace CollegeEventsBlazor.Services;

public class StudentsService
{
    private readonly IHttpClientFactory _factory;
    public StudentsService(IHttpClientFactory factory) => _factory = factory;

    private HttpClient Api => _factory.CreateClient("Api");

    public async Task<List<StudentReadDto>> GetAllAsync()
        => await Api.GetFromJsonAsync<List<StudentReadDto>>("api/Students") ?? new();

    public async Task<bool> UpdateAsync(int id, StudentAdminUpdateDto dto)
        => (await Api.PutAsJsonAsync($"api/Students/{id}", dto)).IsSuccessStatusCode;

    public async Task<bool> ResetPasswordAsync(int id, StudentAdminPasswordDto dto)
        => (await Api.PutAsJsonAsync($"api/Students/{id}/password", dto)).IsSuccessStatusCode;

    public async Task<bool> DeleteAsync(int id)
        => (await Api.DeleteAsync($"api/Students/{id}")).IsSuccessStatusCode;
}
