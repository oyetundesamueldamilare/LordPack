using LordPack.Shared.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace LordPack.Api.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<AudioBook> AudioBooks => Set<AudioBook>();
    public DbSet<Chapter> Chapters => Set<Chapter>();
    public DbSet<Verse> Verses => Set<Verse>();
    public DbSet<Devotional> Devotionals => Set<Devotional>();
    public DbSet<UserPlaylist> Playlists => Set<UserPlaylist>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // AudioBook → Chapter (one-to-many)
        modelBuilder.Entity<AudioBook>()
            .HasMany(b => b.Chapters)
            .WithOne(c => c.AudioBook)
            .HasForeignKey(c => c.AudioBookId)
            .OnDelete(DeleteBehavior.Cascade);

        // Chapter → Verse (one-to-many)
        modelBuilder.Entity<Chapter>()
            .HasMany(c => c.Verses)
            .WithOne()
            .HasForeignKey(v => v.ChapterId)
            .OnDelete(DeleteBehavior.Cascade);

        // UserPlaylist → AppUser (many-to-one)
        modelBuilder.Entity<UserPlaylist>()
            .HasOne(p => p.AppUser)
            .WithMany(u => u.Playlists)
            .HasForeignKey(p => p.AppUserId);
    }
}