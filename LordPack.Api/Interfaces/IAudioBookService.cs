using LordPack.Shared.Models;

namespace LordPack.Api.Interfaces
{
    public interface IAudioBookService
    {
        Task<IEnumerable<AudioBook>> GetBooksAsync(Testament? testament, string version);
        Task<IEnumerable<Chapter>?> GetChaptersByBookIdAsync(int bookId);
        Task<Chapter?> GetChapterDetailsAsync(int chapterId);
    }
}
