using LordPack.Shared.DTOs;
using LordPack.Shared.Models;

namespace LordPack.Api.Interfaces;

public interface IAudioBookService
{
    Task<IEnumerable<AudioBookDto>> GetAudioBookDtosAsync(Testament? testament, string version);
    Task<IEnumerable<Chapter>?> GetChaptersByBookIdAsync(int bookId);
    Task<Chapter?> GetChapterDetailsAsync(int chapterId);
}