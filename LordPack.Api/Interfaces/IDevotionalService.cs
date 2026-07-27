using LordPack.Shared.Models;

namespace LordPack.Api.Interfaces
{
    public interface IDevotionalService
    {
        Task<IEnumerable<Devotional>> GetAllDevotionalsAsync();
        Task<Devotional?> GetDevotionalByIdAsync(int id);
        Task<Devotional?> GetTodayDevotionalAsync();
        Task<Devotional> CreateDevotionalAsync(Devotional devotional);
        Task<bool> UpdateDevotionalAsync(int id, Devotional devotional);
        Task<bool> DeleteDevotionalAsync(int id);
    }
}
