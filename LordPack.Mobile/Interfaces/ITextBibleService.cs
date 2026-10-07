using LordPack.Shared.DTOs;

namespace LordPack.Mobile.Interfaces;

/// <summary>
/// Service that provides Text Bible data with offline-first fallback.
/// Fetches from the API when online and caches results to local SQLite.
/// Returns cached data when offline.
/// </summary>
public interface ITextBibleService
{
    /// <summary>Gets all books, optionally filtered by testament name ("OldTestament" or "NewTestament").</summary>
    Task<List<BibleBookDto>> GetBooksAsync(string? testament = null);

    /// <summary>Gets detailed chapter data including all verses. Uses offline cache as fallback.</summary>
    Task<ChapterDetailDto?> GetChapterDetailAsync(int chapterApiId);
}
