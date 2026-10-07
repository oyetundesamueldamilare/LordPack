using LordPack.Shared.DTOs;

namespace LordPack.Mobile.Interfaces;

public interface IClientAudioBookService
{
    Task<List<AudioBookDto>> GetAudioBooksAsync();
    Task<AudioBookDto?> GetAudioBookByIdAsync(int id);
    Task<ChapterDetailDto?> GetChapterDetailsAsync(int chapterId);
    Task<List<ChapterSummaryDto>> GetChaptersByBookIdAsync(int bookId);
}
