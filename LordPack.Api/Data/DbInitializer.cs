using LordPack.Api.Data;
using LordPack.Shared.Models;

namespace LordPack.Api.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (context.AudioBooks.Any()) return; // Already seeded


        var genesis = new AudioBook
        {
            Name = "Genesis",
            Testament = Testament.OldTestament,
            Version = "KJV",
            TotalChapters = 50,
            Chapters = new List<Chapter>
            {
                new Chapter
                {
                    ChapterNumber = 1,
                    // Public sample MP3 for testing streaming
                    AudioUrl = "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-1.mp3",
                    Duration = TimeSpan.FromMinutes(5)
                }
            }
        };

        var john = new AudioBook
        {
            Name = "John",
            Testament = Testament.NewTestament,
            Version = "KJV",
            TotalChapters = 21,
            Chapters = new List<Chapter>
            {
                new Chapter
                {
                    ChapterNumber = 1,
                    AudioUrl = "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-2.mp3",
                    Duration = TimeSpan.FromMinutes(4)
                }
            }
        };

        var todayDevotional = new Devotional
        {
            Title = "Walking in Faith",
            ScriptureReference = "John 1:1-5",
            BodyText = "In the beginning was the Word, and the Word was with God, and the Word was God...",
            Date = DateTime.UtcNow.Date,
            PrayerPoints = "Lord, open my heart to hear Your word clearly today.",
            AudioUrl = "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-3.mp3"
        };

        await context.AudioBooks.AddRangeAsync(genesis, john);
        await context.Devotionals.AddAsync(todayDevotional);
        await context.SaveChangesAsync();
    }
}

