using LordPack.Shared.Models;

namespace LordPack.Api.Interfaces
{
    public interface IAudioBookRepository
    {
        Task<IEnumerable<AudioBook>> GetBooksAsync(Testament? testament, string version);
        Task<IEnumerable<AudioBook>> GetAllBooksAsync(Testament? testament);
        Task<AudioBook?> GetBookWithChaptersAsync(int bookId);
        Task<Chapter?> GetChapterDetailsAsync(int chapterId);
    }
}
