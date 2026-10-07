using LordPack.Mobile.Interfaces;
using LordPack.Mobile.Models;
using LordPack.Shared.DTOs;
using System.Net.Http.Json;

namespace LordPack.Mobile.Services;

/// <summary>
/// Offline-first Text Bible service.
/// Order of resolution:
///   1. Try API (when network available)
///   2. Persist result to SQLite
///   3. On failure/offline, serve from SQLite cache
/// </summary>
public class TextBibleService : ITextBibleService
{
    private readonly HttpClient _httpClient;
    private readonly LocalBibleDatabase _db;

    public TextBibleService(HttpClient httpClient, LocalBibleDatabase db)
    {
        _httpClient = httpClient;
        _db = db;
    }

    public async Task<List<BibleBookDto>> GetBooksAsync(string? testament = null)
    {
        // 1. Try online
        if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
        {
            try
            {
                var url = testament != null
                    ? $"api/audiobooks/books?testament={testament}"
                    : "api/audiobooks/books";

                var books = await _httpClient.GetFromJsonAsync<List<BibleBookDto>>(url);
                if (books != null && books.Count > 0)
                {
                    await _db.UpsertBooksAsync(books);
                    return books;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[TextBibleService] GetBooks API failed: {ex.Message}");
            }
        }

        // 2. Offline fallback from SQLite
        var localBooks = await _db.GetBooksAsync(testament);
        return localBooks.Select(b => new BibleBookDto
        {
            Id = b.Id,
            Name = b.Name,
            Testament = b.Testament,
            TotalChapters = b.TotalChapters,
            Version = b.Version
        }).ToList();
    }

    public async Task<ChapterDetailDto?> GetChapterDetailAsync(int chapterApiId)
    {
        // 1. Try online
        if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
        {
            try
            {
                var detail = await _httpClient.GetFromJsonAsync<ChapterDetailDto>(
                    $"api/audiobooks/chapters/{chapterApiId}/details");

                if (detail != null)
                {
                    // Cache to SQLite for offline use
                    await _db.UpsertChapterWithVersesAsync(detail);
                    return detail;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[TextBibleService] GetChapter API failed: {ex.Message}");
            }
        }

        // 2. Offline fallback
        var verses = await _db.GetVersesAsync(chapterApiId);
        if (!verses.Any()) return null;

        var cachedChapter = await _db.GetChapterByApiIdAsync(chapterApiId);

        return new ChapterDetailDto
        {
            Id = chapterApiId,
            BookName = cachedChapter?.BookName ?? string.Empty,
            ChapterNumber = cachedChapter?.ChapterNumber ?? 1,
            AudioUrl = cachedChapter?.AudioUrl ?? string.Empty,
            DurationInSeconds = cachedChapter?.DurationInSeconds ?? 0,
            Verses = verses.Select(v => new VerseDto
            {
                VerseNumber = v.VerseNumber,
                Text = v.Text
            }).ToList()
        };
    }
}
