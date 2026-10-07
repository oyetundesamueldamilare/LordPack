using LordPack.Mobile.Models;
using LordPack.Shared.DTOs;
using SQLite;

namespace LordPack.Mobile.Services;

/// <summary>
/// Lightweight SQLite wrapper for offline Bible text storage.
/// Uses sqlite-net-pcl for cross-platform offline persistence.
/// </summary>
public class LocalBibleDatabase
{
    private SQLiteAsyncConnection? _db;

    private async Task<SQLiteAsyncConnection> GetConnectionAsync()
    {
        if (_db != null) return _db;

        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "lordpack_bible.db3");
        _db = new SQLiteAsyncConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create | SQLiteOpenFlags.SharedCache);

        await _db.CreateTableAsync<LocalBook>();
        await _db.CreateTableAsync<LocalChapter>();
        await _db.CreateTableAsync<LocalVerse>();

        // Pre-seed core Bible books if local table is empty
        var bookCount = await _db.Table<LocalBook>().CountAsync();
        if (bookCount == 0)
        {
            await SeedInitialBibleDataAsync(_db);
        }

        return _db;
    }

    private static async Task SeedInitialBibleDataAsync(SQLiteAsyncConnection db)
    {
        var initialBooks = new List<LocalBook>
        {
            // Old Testament
            new() { Id = 1, Name = "Genesis", Testament = "OldTestament", TotalChapters = 50, Version = "KJV" },
            new() { Id = 2, Name = "Exodus", Testament = "OldTestament", TotalChapters = 40, Version = "KJV" },
            new() { Id = 3, Name = "Leviticus", Testament = "OldTestament", TotalChapters = 27, Version = "KJV" },
            new() { Id = 19, Name = "Psalms", Testament = "OldTestament", TotalChapters = 150, Version = "KJV" },
            new() { Id = 20, Name = "Proverbs", Testament = "OldTestament", TotalChapters = 31, Version = "KJV" },
            new() { Id = 23, Name = "Isaiah", Testament = "OldTestament", TotalChapters = 66, Version = "KJV" },

            // New Testament
            new() { Id = 40, Name = "Matthew", Testament = "NewTestament", TotalChapters = 28, Version = "KJV" },
            new() { Id = 41, Name = "Mark", Testament = "NewTestament", TotalChapters = 16, Version = "KJV" },
            new() { Id = 42, Name = "Luke", Testament = "NewTestament", TotalChapters = 24, Version = "KJV" },
            new() { Id = 43, Name = "John", Testament = "NewTestament", TotalChapters = 21, Version = "KJV" },
            new() { Id = 44, Name = "Acts", Testament = "NewTestament", TotalChapters = 28, Version = "KJV" },
            new() { Id = 45, Name = "Romans", Testament = "NewTestament", TotalChapters = 16, Version = "KJV" },
            new() { Id = 66, Name = "Revelation", Testament = "NewTestament", TotalChapters = 22, Version = "KJV" },
        };

        await db.InsertAllAsync(initialBooks);

        // Pre-seed Genesis 1 verses for immediate first-launch offline experience
        var genesis1Chapter = new LocalChapter
        {
            ApiId = 1,
            BookId = 1,
            BookName = "Genesis",
            ChapterNumber = 1,
            AudioUrl = "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-1.mp3",
            DurationInSeconds = 300
        };
        await db.InsertAsync(genesis1Chapter);

        var genesis1Verses = new List<LocalVerse>
        {
            new() { ChapterApiId = 1, VerseNumber = 1, Text = "In the beginning God created the heaven and the earth." },
            new() { ChapterApiId = 1, VerseNumber = 2, Text = "And the earth was without form, and void; and darkness was upon the face of the deep. And the Spirit of God moved upon the face of the waters." },
            new() { ChapterApiId = 1, VerseNumber = 3, Text = "And God said, Let there be light: and there was light." },
            new() { ChapterApiId = 1, VerseNumber = 4, Text = "And God saw the light, that it was good: and God divided the light from the darkness." },
            new() { ChapterApiId = 1, VerseNumber = 5, Text = "And God called the light Day, and the darkness he called Night. And the evening and the morning were the first day." },
        };
        await db.InsertAllAsync(genesis1Verses);
    }

    // ---- Books ----

    public async Task<List<LocalBook>> GetBooksAsync(string? testament = null)
    {
        var conn = await GetConnectionAsync();
        if (!string.IsNullOrEmpty(testament))
            return await conn.Table<LocalBook>().Where(b => b.Testament == testament).OrderBy(b => b.Id).ToListAsync();
        return await conn.Table<LocalBook>().OrderBy(b => b.Id).ToListAsync();
    }

    public async Task UpsertBooksAsync(IEnumerable<BibleBookDto> books)
    {
        var conn = await GetConnectionAsync();
        foreach (var b in books)
        {
            var localBook = new LocalBook
            {
                Id = b.Id,
                Name = b.Name,
                Testament = b.Testament,
                TotalChapters = b.TotalChapters,
                Version = b.Version
            };
            await conn.InsertOrReplaceAsync(localBook);
        }
    }

    // ---- Chapters ----

    public async Task<List<LocalChapter>> GetChaptersByBookIdAsync(int bookId)
    {
        var conn = await GetConnectionAsync();
        return await conn.Table<LocalChapter>().Where(c => c.BookId == bookId).OrderBy(c => c.ChapterNumber).ToListAsync();
    }

    public async Task<LocalChapter?> GetChapterByApiIdAsync(int chapterApiId)
    {
        var conn = await GetConnectionAsync();
        return await conn.Table<LocalChapter>().Where(c => c.ApiId == chapterApiId).FirstOrDefaultAsync();
    }

    // ---- Verses ----

    public async Task<List<LocalVerse>> GetVersesAsync(int chapterApiId)
    {
        var conn = await GetConnectionAsync();
        return await conn.Table<LocalVerse>().Where(v => v.ChapterApiId == chapterApiId).OrderBy(v => v.VerseNumber).ToListAsync();
    }

    public async Task UpsertChapterWithVersesAsync(ChapterDetailDto detail)
    {
        var conn = await GetConnectionAsync();

        // Upsert chapter record
        var existing = await conn.Table<LocalChapter>().Where(c => c.ApiId == detail.Id).FirstOrDefaultAsync();
        if (existing == null)
        {
            await conn.InsertAsync(new LocalChapter
            {
                ApiId = detail.Id,
                BookName = detail.BookName,
                ChapterNumber = detail.ChapterNumber,
                AudioUrl = detail.AudioUrl,
                DurationInSeconds = detail.DurationInSeconds
            });
        }
        else
        {
            existing.BookName = detail.BookName;
            existing.ChapterNumber = detail.ChapterNumber;
            existing.AudioUrl = detail.AudioUrl;
            existing.DurationInSeconds = detail.DurationInSeconds;
            await conn.UpdateAsync(existing);
        }

        // Delete old verses for this chapter then re-insert
        await conn.ExecuteAsync("DELETE FROM LocalVerses WHERE ChapterApiId = ?", detail.Id);
        var verses = detail.Verses.Select(v => new LocalVerse
        {
            ChapterApiId = detail.Id,
            VerseNumber = v.VerseNumber,
            Text = v.Text
        }).ToList();
        await conn.InsertAllAsync(verses);
    }
}
