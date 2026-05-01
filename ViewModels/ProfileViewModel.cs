using System.ComponentModel;
using System.Windows.Input;
using NutriTrack.Data.Repositories;
using NutriTrack.Helpers;
using NutriTrack.Models;

namespace NutriTrack.ViewModels;

public class ProfileViewModel :  BaseViewModel
{
    private readonly INutriRepository _repository;
    private User _user;
    
    public string Name { get => _user.Name; set { _user.Name = value; OnPropertyChanged(); } }
    public double Weight { get => _user.Weight; set { _user.Weight = value; OnPropertyChanged(); } }
    public int Age { get => _user.Age; set { _user.Age = value; OnPropertyChanged(); } }
    public Gender SelectedGender { get => _user.UserGender; set { _user.UserGender = value; OnPropertyChanged(); } }
    public ActivityLevel SelectedActivity { get => _user.Activity; set { _user.Activity = value; OnPropertyChanged(); } }

    public ICommand SaveProfileCommand { get; }

    public ProfileViewModel(INutriRepository repository, User user)
    {
        _repository = repository;
        _user = user;
        SaveProfileCommand = new RelayCommand(async _ => await SaveProfile());
    }

    private async Task SaveProfile()
    {
        await _repository.UpdateUserAsync(_user);
        await _repository.SaveChangesAsync();
    }
}