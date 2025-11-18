using Application.DTOs.Product;
using Application.Mappers;
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

    public async Task<List<ProductReadDto>> GetAllReadAsync()
    {
        var products = await _repo.Query()
            .Include(p => p.Title)
            .AsNoTracking()
            .ToListAsync();

        return products.Select(ProductMapper.ToReadDto).ToList();
    }


    public async Task<ProductReadDto?> GetReadByIdAsync(Guid id)
    {
        var product = await _repo.Query()
            .Include(p => p.Title)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        return product == null ? null : ProductMapper.ToReadDto(product);
    }


    public async Task<List<ProductReadDto>> GetByTitleIdAsync(int titleId)
    {
        var products = await _repo.Query()
            .Include(p => p.Title)
            .AsNoTracking()
            .Where(p => p.TitleId == titleId)
            .ToListAsync();

        return products.Select(ProductMapper.ToReadDto).ToList();
    }


    public async Task<Guid> CreateAsync(int titleId, ProductCreateDto dto)
    {
        var entity = ProductMapper.ToEntity(dto);
        entity.Id = Guid.NewGuid();
        entity.TitleId = titleId;
        entity.CreatedAt = DateTime.UtcNow;

        await _repo.AddAsync(entity);
        await _uow.SaveChangesAsync();

        return entity.Id;
    }


    public async Task<bool> UpdateAsync(Guid id, ProductUpdateDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null)
            return false;

        ProductMapper.UpdateEntity(entity, dto);

        _repo.Update(entity);
        await _uow.SaveChangesAsync();
        return true;
    }


    public async Task<bool> DeleteAsync(Guid id)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity == null)
            return false;

        _repo.Remove(entity);
        await _uow.SaveChangesAsync();
        return true;
    }
}