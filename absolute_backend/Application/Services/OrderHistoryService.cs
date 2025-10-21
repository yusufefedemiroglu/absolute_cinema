using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Application.Services;

public class OrderHistoryService
{
    private readonly AppDbContext _db;

    public OrderHistoryService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<OrderHistory>> GetAllAsync()
    {
        return await _db.OrderHistories
            .OrderByDescending(o => o.CompletedAt)
            .ToListAsync();
    }

    public async Task<OrderHistory?> GetByOrderIdAsync(Guid orderId)
    {
        return await _db.OrderHistories
            .FirstOrDefaultAsync(o => o.OrderId == orderId);
    }
}