using Core.Entities;
using Infrastructure.Data.Repositories.Abstract;
using Infrastructure.Data.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class ProductService : BaseService<Product>
{
    public ProductService(IGenericRepository<Product> repo, IUnitOfWork uow)
        : base(repo, uow)
    {
    }

    public async Task<List<Product>> GetByTitleIdAsync(int titleId)
    {
        return await _repo.Query()
            .AsNoTracking()
            .Where(p => p.TitleId == titleId)
            .ToListAsync();
    }


    public async Task<Guid> CreateAsync(Product p)
    {
        p.Id = Guid.NewGuid();
        p.CreatedAt = DateTime.UtcNow;

        await _repo.AddAsync(p);
        await _uow.SaveChangesAsync();
        return p.Id;
    }

    // 🔹 Ürün güncelle
    public async Task<bool> UpdateAsync(Guid id, Product input)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null)
            return false;

        entity.Name = input.Name;
        entity.Description = input.Description;
        entity.Price = input.Price;
        entity.ImageUrl = input.ImageUrl;
        entity.Stock = input.Stock;
        entity.TitleId = input.TitleId;

        _repo.Update(entity);
        await _uow.SaveChangesAsync();
        return true;
    }
    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null)
            return false;

        _repo.Remove(entity);
        await _uow.SaveChangesAsync();
        return true;
    }
}