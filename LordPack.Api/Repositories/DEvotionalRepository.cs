using LordPack.Api.Data;
using LordPack.Api.Interfaces;
using LordPack.Shared.Models;
using Microsoft.EntityFrameworkCore;    

namespace LordPack.Api.Repositories
{
    public class DevotionalRepository : IDevotionalRepository
    {
        private readonly AppDbContext _context;

        public DevotionalRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Devotional>> GetAllAsync()
        {
            return await _context.Devotionals
                .OrderByDescending(d => d.Date)
                .ToListAsync();
        }

        public async Task<Devotional?> GetByIdAsync(int id)
        {
            return await _context.Devotionals.FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<Devotional?> GetDevotionalByDateAsync(DateTime date)
        {
            return await _context.Devotionals
                .FindAsync();
        }

        public async Task<Devotional?> GetLatestDevotionalAsync()
        {
            return await _context.Devotionals
                .OrderByDescending(d => d.Date)
                .FirstOrDefaultAsync();
        }

        public async Task<Devotional> CreateAsync(Devotional devotional)
        {
            await _context.Devotionals.AddAsync(devotional);
            await _context.SaveChangesAsync();
            return devotional;
        }

        public async Task UpdateAsync(Devotional devotional)
        {
            _context.Devotionals.Update(devotional);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Devotional devotional)
        {
            _context.Devotionals.Remove(devotional);
            await _context.SaveChangesAsync();
        }
    }
}
