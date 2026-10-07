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
                },
                new Chapter
                {
                    ChapterNumber = 2,
                    AudioUrl = "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-4.mp3",
                    Duration = TimeSpan.FromMinutes(4),
                    Verses = new List<Verse>
                    {
                        new Verse { VerseNumber = 1, Text = "Thus the heavens and the earth were finished, and all the host of them." },
                        new Verse { VerseNumber = 2, Text = "And on the seventh day God ended his work which he had made; and he rested on the seventh day from all his work which he had made." },
                        new Verse { VerseNumber = 3, Text = "And God blessed the seventh day, and sanctified it: because that in it he had rested from all his work which God created and made." }
                    }
                }
            }
        };

        var psalms = new AudioBook
        {
            Name = "Psalms",
            Testament = Testament.OldTestament,
            Version = "KJV",
            TotalChapters = 150,
            Chapters = new List<Chapter>
            {
                new Chapter
                {
                    ChapterNumber = 23,
                    AudioUrl = "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-5.mp3",
                    Duration = TimeSpan.FromMinutes(3),
                    Verses = new List<Verse>
                    {
                        new Verse { VerseNumber = 1, Text = "The Lord is my shepherd; I shall not want." },
                        new Verse { VerseNumber = 2, Text = "He maketh me to lie down in green pastures: he leadeth me beside the still waters." },
                        new Verse { VerseNumber = 3, Text = "He restoreth my soul: he leadeth me in the paths of righteousness for his name's sake." },
                        new Verse { VerseNumber = 4, Text = "Yea, though I walk through the valley of the shadow of death, I will fear no evil: for thou art with me; thy rod and thy staff they comfort me." },
                        new Verse { VerseNumber = 5, Text = "Thou preparest a table before me in the presence of mine enemies: thou anointest my head with oil; my cup runneth over." },
                        new Verse { VerseNumber = 6, Text = "Surely goodness and mercy shall follow me all the days of my life: and I will dwell in the house of the Lord for ever." }
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

        var romans = new AudioBook
        {
            Name = "Romans",
            Testament = Testament.NewTestament,
            Version = "KJV",
            TotalChapters = 16,
            Chapters = new List<Chapter>
            {
                new Chapter
                {
                    ChapterNumber = 8,
                    AudioUrl = "https://www.soundhelix.com/examples/mp3/SoundHelix-Song-6.mp3",
                    Duration = TimeSpan.FromMinutes(5),
                    Verses = new List<Verse>
                    {
                        new Verse { VerseNumber = 1, Text = "There is therefore now no condemnation to them which are in Christ Jesus, who walk not after the flesh, but after the Spirit." },
                        new Verse { VerseNumber = 2, Text = "For the law of the Spirit of life in Christ Jesus hath made me free from the law of sin and death." },
                        new Verse { VerseNumber = 28, Text = "And we know that all things work together for good to them that love God, to them who are the called according to his purpose." },
                        new Verse { VerseNumber = 31, Text = "What shall we then say to these things? If God be for us, who can be against us?" }
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

        await context.AudioBooks.AddRangeAsync(genesis, psalms, john, romans);
        await context.Devotionals.AddAsync(todayDevotional);
        await context.SaveChangesAsync();
    }
}