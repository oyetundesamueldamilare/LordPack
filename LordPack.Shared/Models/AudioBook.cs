using System;
using System.Collections.Generic;
using System.Text;

namespace LordPack.Shared.Models
{
    public class AudioBook
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty; // e.g., "Genesis", "John"
        public Testament Testament { get; set; }
        public string Version { get; set; } = "KJV"; // KJV, WEB
        public int TotalChapters { get; set; }

        public List<Chapter> Chapters { get; set; } = new();
    }
}
