using LordPack.Shared.Models;

namespace LordPack.Api.Interfaces
{
   
        public interface IAudioBookRepository
        {
            Task<IEnumerable<AudioBook>> GetBooksAsync(Testament? testament, string version);
            Task<AudioBook?> GetBookWithChaptersAsync(int bookId);
            Task<Chapter?> GetChapterDetailsAsync(int chapterId);
        }
    
}
