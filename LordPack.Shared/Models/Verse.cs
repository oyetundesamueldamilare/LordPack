using System;
using System.Collections.Generic;
using System.Text;

namespace LordPack.Shared.Models
{
    public class Verse
    {
        public int Id { get; set; }
        public int ChapterId { get; set; }
        public int VerseNumber { get; set; }
        public string Text { get; set; } = string.Empty; // e.g., "In the beginning God created..."

        // Optional: Timestamp for audio synchronization / verse highlighting
        public TimeSpan? AudioStartTimestamp { get; set; }
    }
}
