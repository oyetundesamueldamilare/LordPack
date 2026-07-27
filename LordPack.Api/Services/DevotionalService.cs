using LordPack.Api.Interfaces;
using LordPack.Shared.Models;

namespace LordPack.Api.Services;



public class DevotionalService : IDevotionalService
{
    private readonly IDevotionalRepository _repository;

    public DevotionalService(IDevotionalRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Devotional>> GetAllDevotionalsAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<Devotional?> GetDevotionalByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Devotional?> GetTodayDevotionalAsync()
    {
        var today = DateTime.UtcNow.Date;
        var devotional = await _repository.GetDevotionalByDateAsync(today);

        // Fallback to latest available devotional if today's is missing
        if (devotional == null)
        {
            devotional = await _repository.GetLatestDevotionalAsync();
        }

        return devotional;
    }

    public async Task<Devotional> CreateDevotionalAsync(Devotional devotional)
    {
        // Ensure date defaults to today if not explicitly set
        if (devotional.Date == default)
        {
            devotional.Date = DateTime.UtcNow.Date;
        }

        return await _repository.CreateAsync(devotional);
    }

    public async Task<bool> UpdateDevotionalAsync(int id, Devotional devotional)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return false;

        existing.Title = devotional.Title;
        existing.ScriptureReference = devotional.ScriptureReference;
        existing.BodyText = devotional.BodyText;
        existing.AudioUrl = devotional.AudioUrl;
        existing.Date = devotional.Date;
        existing.PrayerPoints = devotional.PrayerPoints;

        await _repository.UpdateAsync(existing);
        return true;
    }

    public async Task<bool> DeleteDevotionalAsync(int id)
    {
        var existing = await _repository.GetByIdAsync(id);
        if (existing == null) return false;

        await _repository.DeleteAsync(existing);
        return true;
    }
}