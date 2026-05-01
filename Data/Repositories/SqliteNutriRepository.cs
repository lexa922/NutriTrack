using Microsoft.EntityFrameworkCore;
using NutriTrack.Models;

namespace NutriTrack.Data.Repositories;

public class SqliteNutriRepository : INutriRepository
{
    private readonly AppDbContext _context;

    public SqliteNutriRepository(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<Product>> GetAllProductsAsync() 
        => await _context.Products.ToListAsync();

    public async Task AddProductAsync(Product product) 
        => await _context.Products.AddAsync(product);

    public async Task<Product?> GetProductByIdAsync(int id) 
        => await _context.Products.FindAsync(id);

    
    public async Task AddFoodLogAsync(FoodLog log) 
        => await _context.FoodLogs.AddAsync(log);

    public async Task<IEnumerable<FoodLog>> GetLogsByDateAsync(DateTime date, int userId)
    {
        return await _context.FoodLogs
            .Include(f => f.Product)
            .Where(f => f.UserId == userId && f.ConsumptionDate.Date == date.Date)
            .ToListAsync();
    }

    public async Task DeleteLogAsync(int logId)
    {
        var log = await _context.FoodLogs.FindAsync(logId);
        if (log != null) _context.FoodLogs.Remove(log);
    }
    
    public async Task<User?> GetUserAsync(int userId) 
        => await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);

    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}