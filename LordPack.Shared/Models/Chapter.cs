using System;
using System.Collections.Generic;
using System.Text;

namespace LordPack.Shared.Models
{
    public class Chapter
    {
      
       
            public int Id { get; set; }
            public int AudioBookId { get; set; }
            public int ChapterNumber { get; set; }
            public string AudioUrl { get; set; } = string.Empty; // Streaming URL
            public TimeSpan Duration { get; set; }

            public AudioBook? AudioBook { get; set; }

            // Connected Bible Text
            public List<Verse> Verses { get; set; } = new();
        }
    
}
