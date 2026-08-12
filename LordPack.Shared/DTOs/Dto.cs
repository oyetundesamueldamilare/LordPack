using System;
using System.Collections.Generic;
using System.Text;

namespace LordPack.Shared.DTOs;

public class RegisterRequestDto
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
public class RegisterDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class LoginRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
public class LoginDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsGuest { get; set; }
        public DateTime Expiration { get; set; }
    }

public class AudioBookDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Narrator { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CoverImageUrl { get; set; } = string.Empty;
    public string AudioUrl { get; set; } = string.Empty;
    public double DurationInSeconds { get; set; }
    public string Category { get; set; } = string.Empty;
    public DateTime PublishedDate { get; set; }
}

public class ChapterDetailDto
{
    public int Id { get; set; }
    public int ChapterNumber { get; set; }
    public string BookName { get; set; } = string.Empty;
    public string AudioUrl { get; set; } = string.Empty;
    public double DurationInSeconds { get; set; }
    public List<VerseDto> Verses { get; set; } = new();
}

public class VerseDto
{
    public int VerseNumber { get; set; }
    public string Text { get; set; } = string.Empty;
}