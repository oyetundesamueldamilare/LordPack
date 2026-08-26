using LordPack.Api.Data;
using LordPack.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace LordPack.Api.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        if (await context.AudioBooks.AnyAsync()) return; // Already seeded

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
                    AudioUrl = "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-1.mp3",
                    Duration = TimeSpan.FromMinutes(5),
                    Verses = new List<Verse>
                    {
                        new Verse { VerseNumber = 1, Text = "In the beginning God created the heaven and the earth." },
                        new Verse { VerseNumber = 2, Text = "And the earth was without form, and void; and darkness was upon the face of the deep. And the Spirit of God moved upon the face of the waters." },
                        new Verse { VerseNumber = 3, Text = "And God said, Let there be light: and there was light." },
                        new Verse { VerseNumber = 4, Text = "And God saw the light, that it was good: and God divided the light from the darkness." },
                        new Verse { VerseNumber = 5, Text = "And God called the light Day, and the darkness he called Night. And the evening and the morning were the first day." }
                    }
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
                    Duration = TimeSpan.FromMinutes(4),
                    Verses = new List<Verse>
                    {
                        new Verse { VerseNumber = 1, Text = "In the beginning was the Word, and the Word was with God, and the Word was God." },
                        new Verse { VerseNumber = 2, Text = "The same was in the beginning with God." },
                        new Verse { VerseNumber = 3, Text = "All things were made by him; and without him was not any thing made that was made." },
                        new Verse { VerseNumber = 4, Text = "In him was life; and the life was the light of men." },
                        new Verse { VerseNumber = 5, Text = "And the light shineth in darkness; and the darkness comprehended it not." }
                    }
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