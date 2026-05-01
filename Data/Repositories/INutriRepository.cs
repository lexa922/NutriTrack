using NutriTrack.Models;

namespace NutriTrack.Data.Repositories;

public interface INutriRepository
{
    Task<IEnumerable<Product>> GetAllProductsAsync();
    Task<Product?> GetProductByIdAsync(int id);
    Task AddProductAsync(Product product);
    
    Task AddFoodLogAsync(FoodLog log);
    Task<IEnumerable<FoodLog>> GetLogsByDateAsync(DateTime date, int userId);
    Task DeleteLogAsync(int logId);
    
    Task<User?> GetUserAsync(int userId);
    Task UpdateUserAsync(User user);
        
    Task SaveChangesAsync();
}