using Core;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class GenreService
{
    private readonly AppDbContext _db;

    public GenreService(AppDbContext db)
    {
        _db = db;
    }

    // Tüm türleri getir
    public async Task<List<Genre>> GetAllGenresAsync()
    {
        return await _db.Genres
            .OrderBy(g => g.Name)
            .ToListAsync();
    }

    // ID’ye göre tür bul
    public async Task<Genre?> GetByIdAsync(int id)
    {
        return await _db.Genres.FindAsync(id);
    }

    // Arama (örneğin "Action")
    public async Task<List<Genre>> SearchAsync(string query)
    {
        return await _db.Genres
            .Where(g => g.Name.Contains(query))
            .OrderBy(g => g.Name)
            .ToListAsync();
    }
}
