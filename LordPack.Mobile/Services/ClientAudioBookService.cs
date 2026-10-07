using LordPack.Mobile.Interfaces;
using LordPack.Shared.DTOs;
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
        catch
        {
            return new List<AudioBookDto>();
        }
    }

    public async Task<AudioBookDto?> GetAudioBookByIdAsync(int id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<AudioBookDto>($"api/audiobooks/{id}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<ChapterDetailDto?> GetChapterDetailsAsync(int chapterId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<ChapterDetailDto>($"api/audiobooks/chapters/{chapterId}/details");
        }
        catch
        {
            return null;
        }
    }

    public async Task<List<ChapterSummaryDto>> GetChaptersByBookIdAsync(int bookId)
    {
        try
        {
            var result = await _httpClient.GetFromJsonAsync<List<ChapterSummaryDto>>($"api/audiobooks/{bookId}/chapters");
            return result ?? new List<ChapterSummaryDto>();
        }
        catch
        {
            return new List<ChapterSummaryDto>();
        }
    }
}