using LordPack.Shared.DTOs;
using LordPack.Shared.Models;

namespace LordPack.Api.Interfaces;

public interface IAudioBookService
{
    Task<IEnumerable<AudioBookDto>> GetAudioBookDtosAsync(Testament? testament, string version);
    Task<IEnumerable<ChapterSummaryDto>?> GetChaptersByBookIdAsync(int bookId);
    Task<ChapterDetailDto?> GetChapterDetailsAsync(int chapterId);
    Task<IEnumerable<BibleBookDto>> GetBibleBooksAsync(Testament? testament);
}