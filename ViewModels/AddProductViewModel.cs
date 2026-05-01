using System.Windows.Input;
using NutriTrack.Data.Repositories;
using NutriTrack.Helpers;
using NutriTrack.Models;

namespace NutriTrack.ViewModels;

public class AddProductViewModel : BaseViewModel
{
    private readonly INutriRepository _repository;
    private string _name = "";
    private double _calories;
    private double _proteins;
    private double _fats;
    private double _carbs;
    private string _selectedCategory = "Загальне";

    public string Name { get => _name; set => SetProperty(ref _name, value); }
    public double Calories { get => _calories; set => SetProperty(ref _calories, value); }
    public double Proteins { get => _proteins; set => SetProperty(ref _proteins, value); }
    public double Fats { get => _fats; set => SetProperty(ref _fats, value); }
    public double Carbs { get => _carbs; set => SetProperty(ref _carbs, value); }
    public List<string> Categories { get; } = new() { "Овочі", "Фрукти", "М'ясо", "Молочні продукти", "Напої", "Загальне" };
    
    public string SelectedCategory 
    { 
        get => _selectedCategory; 
        set => SetProperty(ref _selectedCategory, value); 
    }

    public ICommand SaveCommand { get; }
    
    public event Action<Product>? ProductAdded;

    public AddProductViewModel(INutriRepository repository)
    {
        _repository = repository;
        SaveCommand = new RelayCommand(async _ => await SaveProduct(), _ => !string.IsNullOrWhiteSpace(Name));
    }

    private async Task SaveProduct()
    {
        var product = new Product
        {
            Name = Name,
            Calories = Calories,
            Proteins = Proteins,
            Fats = Fats,
            Carbohydrates = Carbs,
            Category = SelectedCategory
        };

        await _repository.AddProductAsync(product);
        await _repository.SaveChangesAsync();
            
        ProductAdded?.Invoke(product);
    }
}