using System;
using System.Collections.Generic;
using System.Text;

namespace LordPack.Shared.Models
{
  
    public class Devotional
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string ScriptureReference { get; set; } = string.Empty; // e.g., "Psalm 23:1-6"
        public string BodyText { get; set; } = string.Empty;
        public string? AudioUrl { get; set; } // Optional devotional audio track
        public DateTime Date { get; set; } // Daily target date
        public string PrayerPoints { get; set; } = string.Empty;
    }
}
