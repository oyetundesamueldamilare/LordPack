using LordPack.Api.Interfaces;
using LordPack.Mobile.Interfaces;
using LordPack.Shared.DTOs; // Assuming DTOs like AudioBookDto are shared
using System.Net.Http.Json;

namespace LordPack.Mobile.Services;



public class ClientAudioBookService : IClientAudioBookService
{
    private readonly HttpClient _httpClient;

    public ClientAudioBookService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<AudioBookDto>> GetAudioBooksAsync()
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<AudioBookDto>>("api/audiobooks");
            return result ?? new List<AudioBookDto>();
        }
        catch (Exception)
        {
            // Log or handle network error
            return new List<AudioBookDto>();
        }
    }

    public async Task<AudioBookDto?> GetAudioBookByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<AudioBookDto>($"api/audiobooks/{id}");
        }
        catch (Exception)
        {
            return null;
        }
    }
}