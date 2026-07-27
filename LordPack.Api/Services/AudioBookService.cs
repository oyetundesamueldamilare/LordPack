using LordPack.Api.Interfaces;
using LordPack.Shared.Models;

namespace LordPack.Api.Services;


public class AudioBookService : IAudioBookService
{
    private readonly IAudioBookRepository _repository;

    public AudioBookService(IAudioBookRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<AudioBook>> GetBooksAsync(Testament? testament, string version)
    {
        return await _repository.GetBooksAsync(testament, version);
    }

    public async Task<IEnumerable<Chapter>?> GetChaptersByBookIdAsync(int bookId)
    {
        var book = await _repository.GetBookWithChaptersAsync(bookId);
        return book?.Chapters.OrderBy(c => c.ChapterNumber);
    }

    public async Task<Chapter?> GetChapterDetailsAsync(int chapterId)
    {
        return await _repository.GetChapterDetailsAsync(chapterId);
    }
}