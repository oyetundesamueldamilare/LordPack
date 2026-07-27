namespace LordPack.Shared.Models
{
    public class UserPlaylist
    {
        public int Id { get; set; }
        public string AppUserId { get; set; } = string.Empty; // Maps to IdentityUser string Key
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public AppUser? AppUser { get; set; }
        public List<Chapter> Chapters { get; set; } = new();
    }
}