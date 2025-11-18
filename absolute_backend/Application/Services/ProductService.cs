using Application.DTOs.Product;
using Core.Entities;
using Infrastructure.Data.Repositories.Abstract;
using Infrastructure.Data.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class ProductService : BaseService<Product>
{
    private readonly IGenericRepository<Title> _titleRepo;

    public ProductService(
        IGenericRepository<Product> repo,
        IUnitOfWork uow,
        IGenericRepository<Title> titleRepo)
        : base(repo, uow)
    {
        _titleRepo = titleRepo;
    }

    //get by TitleId
    public async Task<List<Product>> GetByTitleIdAsync(int titleId)
    {
        return await _repo.Query()
            .AsNoTracking()
            .Where(p => p.TitleId == titleId)
            .ToListAsync();
    }

    //create with dto
    public async Task<Guid> CreateAsync(int titleId, ProductCreateDto dto)
    {
        //check if title exists
        var titleExists = await _titleRepo.Query().AnyAsync(t => t.Id == titleId);
        if (!titleExists)
            throw new Exception("Title not found.");

        var product = new Product
        {
            Id = Guid.NewGuid(),
            TitleId = titleId,
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            ImageUrl = dto.ImageUrl,
            Stock = dto.Stock,
            CreatedAt = DateTime.UtcNow
        };

        await _repo.AddAsync(product);
        await _uow.SaveChangesAsync();

        return product.Id;
    }

    //update with dto
    public async Task<bool> UpdateAsync(Guid id, ProductUpdateDto dto)
    {
        var entity = await _repo.GetByIdAsync(id);
        if (entity is null)
            return false;
        //if titleId is being updated, check existence(validation should handle most cases)
        if (dto.TitleId.HasValue)
        {
            var exists = await _titleRepo.Query().AnyAsync(t => t.Id == dto.TitleId.Value);
            if (!exists)
                throw new Exception("TitleId does not exist.");
            entity.TitleId = dto.TitleId.Value;
        }
        // optional fields 
        entity.Name = dto.Name ?? entity.Name;
        entity.Description = dto.Description ?? entity.Description;
        entity.Price = dto.Price ?? entity.Price;
        entity.ImageUrl = dto.ImageUrl ?? entity.ImageUrl;
        entity.Stock = dto.Stock ?? entity.Stock;

        _repo.Update(entity);
        await _uow.SaveChangesAsync();

        return true;
    }

    // delete by ID
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