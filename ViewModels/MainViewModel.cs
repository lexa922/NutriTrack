using System.Collections.ObjectModel;
using System.Windows.Input;
using NutriTrack.Data.Repositories;
using NutriTrack.Helpers;
using NutriTrack.Models;
using NutriTrack.Services;
using NutriTrack.Services.Strategies;
using NutriTrack.Views;

namespace NutriTrack.ViewModels;

public class MainViewModel : BaseViewModel
{
    private readonly INutriRepository _repository;
        private readonly CalorieCalculator _calculator;
        
        private readonly IDispatcherService _dispatcherService;
        public ICommand AddFoodCommand { get; }
        public ICommand OpenAddProductWindowCommand { get; }
        public ICommand DeleteLogCommand { get; }
        public ICommand UpdateLogCommand { get; }
        public ICommand CancelSelectionCommand { get; }
        
        private double _dailyGoal;
        private double _currentCalories;
        private User? _currentUser;
        private Product? _selectedProduct;
        private double? _inputWeight;
        private string _searchText = string.Empty;
        
        private double _totalProteins;
        private double _totalFats;
        private double _totalCarbs;
        
        private FoodLog _selectedLog;

        public double TotalProteins { get => _totalProteins; set => SetProperty(ref _totalProteins, value); }
        public double TotalFats { get => _totalFats; set => SetProperty(ref _totalFats, value); }
        public double TotalCarbs { get => _totalCarbs; set => SetProperty(ref _totalCarbs, value); }
        public double CurrentCalories { get => _currentCalories; set => SetProperty(ref _currentCalories, value); }

        public ObservableCollection<Product> FilteredProducts { get; } = new();

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (SetProperty(ref _searchText, value))
                {
                    ApplyFilter();
                }
            }
        }
        
        public FoodLog SelectedLog 
        { 
            get => _selectedLog; 
            set
            {
                if (SetProperty(ref _selectedLog, value))
                {
                    if (value != null)
                    {
                        InputWeight = value.ServingSizeGrams;
                        
                        SelectedProduct = FilteredProducts.FirstOrDefault(p => p.Id == value.ProductId);
                    }
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }
        
        public ObservableCollection<Product> AvailableProducts { get; } = new();
        
        
        public Product? SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                if (SetProperty(ref _selectedProduct, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public double? InputWeight
        {
            get => _inputWeight;
            set
            {
                if (SetProperty(ref _inputWeight, value))
                {
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }
        
        public ObservableCollection<FoodLog> TodayLogs { get; } = new();

        public double DailyGoal
        {
            get => _dailyGoal;
            set => SetProperty(ref _dailyGoal, value);
        }

        public MainViewModel(INutriRepository repository, User user, IDispatcherService  dispatcherService, CalorieCalculator calculator)
        {
            _repository = repository;
            _calculator = calculator;
            _calculator.SetStrategy(new MifflinStJeorStrategy());
            _dispatcherService = dispatcherService;
            
            _currentUser = user;
            
            AddFoodCommand = new RelayCommand(async _ => await AddFoodEntry(), _ => CanAddFood());
            OpenAddProductWindowCommand = new RelayCommand(_ => OpenAddProductWindow());
            DeleteLogCommand = new RelayCommand(async _ => await DeleteLog(), _ => SelectedLog != null);
            UpdateLogCommand = new RelayCommand(async _ => await UpdateLog(), _ => SelectedLog != null);
            CancelSelectionCommand = new RelayCommand(_ => SelectedLog = null);
            
            Task.Run(InitializeAsync);
        }
        
        private bool CanAddFood()
        {
            bool hasProduct = SelectedProduct != null;
            bool hasWeight = InputWeight > 0;
            
            return hasProduct && hasWeight;
        }
        
        private void ApplyFilter()
        {
            var search = (SearchText ?? "").ToLower().Trim();
    
            var filtered = AvailableProducts
                .Where(p => string.IsNullOrWhiteSpace(search) || p.Name.ToLower().Contains(search))
                .ToList();

            FilteredProducts.Clear();
            foreach (var p in filtered) 
            {
                FilteredProducts.Add(p);
            }
        }
        
        private void OpenAddProductWindow()
        {
            var vm = new AddProductViewModel(_repository);
            var win = new AddProductWindow { DataContext = vm };
            
            vm.ProductAdded += (newProduct) => {
                AvailableProducts.Add(newProduct);
                win.Close();
            };

            win.ShowDialog();
        }

        private async Task AddFoodEntry()
        {
            var newLog = new FoodLog
            {
                UserId = _currentUser.Id,
                ProductId = SelectedProduct.Id,
                ConsumptionDate = DateTime.Now,
                ServingSizeGrams = InputWeight.GetValueOrDefault(),
                Product = SelectedProduct 
            };

            await _repository.AddFoodLogAsync(newLog);
            await _repository.SaveChangesAsync();
            
            _dispatcherService.Invoke(() => {
                TodayLogs.Add(newLog);
                UpdateTotals();
            });
        }
        
        private async Task DeleteLog()
        {
            if (SelectedLog == null) return;
            
            await _repository.DeleteLogAsync(SelectedLog.Id);
            await _repository.SaveChangesAsync();
            
            TodayLogs.Remove(SelectedLog);
            
            UpdateTotals();
        }
        
        private async Task UpdateLog()
        {
            double weight = InputWeight.GetValueOrDefault();
            
                SelectedLog.ServingSizeGrams = weight;
                SelectedLog.ProductId = SelectedProduct.Id;
                SelectedLog.Product = SelectedProduct;
                
                await _repository.SaveChangesAsync();
                
                var index = TodayLogs.IndexOf(SelectedLog);
                if (index != -1)
                {
                    var updatedLog = SelectedLog;
                    TodayLogs.RemoveAt(index);
                    TodayLogs.Insert(index, updatedLog);
                }

                UpdateTotals();
                
                SelectedLog = null;
        }
    

        private async Task InitializeAsync()
        {
            LoadDailyGoal();
            await LoadAvailableProductsAsync();
            await  LoadTodayLogsAsync();
            UpdateTotals();
        }
        private void LoadDailyGoal()
        {
            DailyGoal = _calculator.CalculateDailyGoal(_currentUser);
        }

        private async Task LoadAvailableProductsAsync()
        {
            var products = await _repository.GetAllProductsAsync();
            AvailableProducts.Clear();
            foreach (var p in products) 
                AvailableProducts.Add(p);
        }

        private async Task LoadTodayLogsAsync()
        {
            var logs = await _repository.GetLogsByDateAsync(DateTime.Now, _currentUser.Id);
            TodayLogs.Clear();
            foreach (var log in logs) TodayLogs.Add(log);
        }

        private void UpdateTotals()
        {
            CurrentCalories = TodayLogs.Sum(log => log.TotalCalories);
            TotalProteins = TodayLogs.Sum(log => log.TotalProteins);
            TotalFats = TodayLogs.Sum(log => log.TotalFats);
            TotalCarbs = TodayLogs.Sum(log => log.TotalCarbs);
        }
    
}