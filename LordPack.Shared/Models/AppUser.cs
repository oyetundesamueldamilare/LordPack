using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;




namespace LordPack.Shared.Models;

public class AppUser : IdentityUser
{
    public string FullName { get; set; } = string.Empty;
    public bool IsGuest { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public List<UserPlaylist> Playlists { get; set; } = new();
}

