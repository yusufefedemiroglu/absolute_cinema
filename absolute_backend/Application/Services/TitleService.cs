using Core;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class TitleService
{
    private readonly AppDbContext _db;

    public TitleService(AppDbContext db)
    {
        _db = db;
    }

    // get all movies
    public async Task<List<Title>> GetAllTitlesAsync()
    {
        return await _db.Titles.ToListAsync();
    }

    // get one movie
    public async Task<Title?> GetByIdAsync(int id)
    {
        return await _db.Titles.FindAsync(id);
    }

    // search func
    public async Task<List<Title>> SearchAsync(string query)
    {
        return await _db.Titles
            .Where(t => t.Name.Contains(query))
            .ToListAsync();
    }
}
