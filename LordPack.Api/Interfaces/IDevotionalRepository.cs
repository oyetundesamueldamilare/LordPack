using LordPack.Shared.Models;

namespace LordPack.Api.Interfaces
{
    public interface IDevotionalRepository
    {
        Task<IEnumerable<Devotional>> GetAllAsync();
        Task<Devotional?> GetByIdAsync(int id);
        Task<Devotional?> GetDevotionalByDateAsync(DateTime date);
        Task<Devotional?> GetLatestDevotionalAsync();
        Task<Devotional> CreateAsync(Devotional devotional);
        Task UpdateAsync(Devotional devotional);
        Task DeleteAsync(Devotional devotional);
    }
}
