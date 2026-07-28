using LordPack.Shared.DTOs;

namespace LordPack.Api.Interfaces
{
    public interface IClientAudioBookService
    {
        Task<List<AudioBookDto>> GetAudioBooksAsync();
        Task<AudioBookDto?> GetAudioBookByIdAsync(int id);
    }
}
