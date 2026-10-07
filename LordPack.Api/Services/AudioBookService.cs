using LordPack.Api.Interfaces;
using LordPack.Shared.DTOs;
using LordPack.Shared.Models;

namespace LordPack.Api.Services;

public class AudioBookService : IAudioBookService
{
    private readonly IAudioBookRepository _repository;

    public AudioBookService(IAudioBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<AudioBookDto>> GetAudioBookDtosAsync(Testament? testament, string version)
    {
        var books = await _repository.GetBooksAsync(testament, version);

        return books.SelectMany(book => (book.Chapters ?? new List<Chapter>()).Select(chapter => new AudioBookDto
        {
            Id = chapter.Id,
            Title = $"{book.Name} - Chapter {chapter.ChapterNumber}",
            Author = $"{book.Version} ({book.Testament})",
            Description = $"{book.Version} Audio for {book.Name} Chapter {chapter.ChapterNumber}",
            CoverImageUrl = "https://picsum.photos/id/1025/400/400",
            AudioUrl = chapter.AudioUrl,
            DurationInSeconds = (int)chapter.Duration.TotalSeconds
        }));
    }

    public async Task<IEnumerable<ChapterSummaryDto>?> GetChaptersByBookIdAsync(int bookId)
    {
        var book = await _repository.GetBookWithChaptersAsync(bookId);
        return book?.Chapters.OrderBy(c => c.ChapterNumber).Select(c => new ChapterSummaryDto
        {
            Id = c.Id,
            ChapterNumber = c.ChapterNumber,
            AudioUrl = c.AudioUrl,
            DurationInSeconds = c.Duration.TotalSeconds
        });
    }

    public async Task<ChapterDetailDto?> GetChapterDetailsAsync(int chapterId)
    {
        var chapter = await _repository.GetChapterDetailsAsync(chapterId);
        if (chapter == null) return null;

        return new ChapterDetailDto
        {
            Id = chapter.Id,
            ChapterNumber = chapter.ChapterNumber,
            BookName = chapter.AudioBook?.Name ?? string.Empty,
            AudioUrl = chapter.AudioUrl,
            DurationInSeconds = chapter.Duration.TotalSeconds,
            Verses = chapter.Verses?.Select(v => new VerseDto
            {
                VerseNumber = v.VerseNumber,
                Text = v.Text
            }).OrderBy(v => v.VerseNumber).ToList() ?? new()
        };
    }

    public async Task<IEnumerable<BibleBookDto>> GetBibleBooksAsync(Testament? testament)
    {
        var books = await _repository.GetAllBooksAsync(testament);
        return books.Select(b => new BibleBookDto
        {
            Id = b.Id,
            Name = b.Name,
            Testament = b.Testament.ToString(),
            TotalChapters = b.TotalChapters,
            Version = b.Version
        });
    }
}
