using System.Collections.ObjectModel;
using System.Windows.Input;
using NutriTrack.Data.Repositories;
using NutriTrack.Helpers;
using NutriTrack.Models;
using NutriTrack.Services;
using NutriTrack.Services.Strategies;

namespace NutriTrack.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly INutriRepository _repository;
        private readonly CalorieCalculator _calculator;
        public ICommand AddFoodCommand { get; }
        
        private double _dailyGoal;
        private double _currentCalories;
        private User? _currentUser;
        private Product? _selectedProduct;
        private double _inputWeight;
        
        public ObservableCollection<Product> AvailableProducts { get; } = new();
        
        public Product? SelectedProduct
        {
            get => _selectedProduct;
            set => SetProperty(ref _selectedProduct, value);
        }

        public double InputWeight
        {
            get => _inputWeight;
            set => SetProperty(ref _inputWeight, value);
        }
        
        public ObservableCollection<FoodLog> TodayLogs { get; } = new();

        public double DailyGoal
        {
            get => _dailyGoal;
            set => SetProperty(ref _dailyGoal, value);
        }

        public double CurrentCalories
        {
            get => _currentCalories;
            set => SetProperty(ref _currentCalories, value);
        }

        public MainViewModel(INutriRepository repository, User user)
        {
            _repository = repository;
            _calculator = new CalorieCalculator();
            _calculator.SetStrategy(new MifflinStJeorStrategy());
            
            _currentUser = user;
            
            AddFoodCommand = new RelayCommand(async (p) => await AddFoodEntry(), (p) => CanAddFood());
            
            Task.Run(InitializeAsync);
        }
        
        private bool CanAddFood() 
        {
            return SelectedProduct != null && InputWeight > 0;
        }

        private async Task AddFoodEntry()
        {
            var newLog = new FoodLog
            {
                UserId = _currentUser.Id,
                ProductId = SelectedProduct.Id,
                ConsumptionDate = DateTime.Now,
                ServingSizeGrams = InputWeight
            };

            await _repository.AddFoodLogAsync(newLog);
            await _repository.SaveChangesAsync();
            
            TodayLogs.Add(newLog);
            UpdateTotals();
        }

        private async Task InitializeAsync()
        {
            if (_currentUser != null)
            {
                DailyGoal = _calculator.CalculateDailyGoal(_currentUser);
        
                var logs = await _repository.GetLogsByDateAsync(DateTime.Now, _currentUser.Id);
        
                var products = await _repository.GetAllProductsAsync();
        
                App.Current.Dispatcher.Invoke(() => {
                    TodayLogs.Clear();
                    foreach (var log in logs) TodayLogs.Add(log);
            
                    AvailableProducts.Clear();
                    foreach (var p in products) AvailableProducts.Add(p);
            
                    UpdateTotals();
                });
            }
        }

        private void UpdateTotals()
        {
            CurrentCalories = TodayLogs.Sum(l => (l.Product.Calories * l.ServingSizeGrams) / 100);
        }
}