using LordPack.Api.Data;
using LordPack.Api.Interfaces;
using LordPack.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace LordPack.Api.Repositories
{
    public class AudioBookRepository : IAudioBookRepository
    {
        private readonly AppDbContext _context;

        public AudioBookRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AudioBook>> GetBooksAsync(Testament? testament, string version)
        {
            var query = _context.AudioBooks.Where(b => b.Version == version);

            if (testament.HasValue)
            {
                query = query.Where(b => b.Testament == testament.Value);
            }

            return await query.ToListAsync();
        }

        public async Task<AudioBook?> GetBookWithChaptersAsync(int bookId)
        {
            return await _context.AudioBooks
                .Include(b => b.Chapters)
                .FirstOrDefaultAsync(b => b.Id == bookId);
        }

        public async Task<Chapter?> GetChapterDetailsAsync(int chapterId)
        {
            return await _context.Chapters
                .Include(c => c.AudioBook)
                .Include(c => c.Verses)
                .FirstOrDefaultAsync(c => c.Id == chapterId);
        }
    }
}
