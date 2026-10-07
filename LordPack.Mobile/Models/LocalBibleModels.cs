using SQLite;

namespace LordPack.Mobile.Models;

/// <summary>Local SQLite entity for offline Bible book caching.</summary>
[Table("LocalBooks")]
public class LocalBook
{
    [PrimaryKey]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Testament { get; set; } = string.Empty; // "OldTestament" | "NewTestament"
    public int TotalChapters { get; set; }
    public string Version { get; set; } = "KJV";
}

/// <summary>Local SQLite entity for a Bible chapter (audio and text metadata).</summary>
[Table("LocalChapters")]
public class LocalChapter
{
    [PrimaryKey, AutoIncrement]
    public int LocalId { get; set; }
    public int ApiId { get; set; }       // matches Chapter.Id on the server
    public int BookId { get; set; }
    public string BookName { get; set; } = string.Empty;
    public int ChapterNumber { get; set; }
    public string AudioUrl { get; set; } = string.Empty;
    public double DurationInSeconds { get; set; }
}

/// <summary>Local SQLite entity for an individual Bible verse.</summary>
[Table("LocalVerses")]
public class LocalVerse
{
    [PrimaryKey, AutoIncrement]
    public int LocalId { get; set; }
    public int ChapterApiId { get; set; } // matches Chapter.Id on the server
    public int VerseNumber { get; set; }
    public string Text { get; set; } = string.Empty;
}
