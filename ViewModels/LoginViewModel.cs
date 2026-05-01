using System.Windows.Input;
using NutriTrack.Data.Repositories;
using NutriTrack.Helpers;
using NutriTrack.Models;

namespace NutriTrack.ViewModels;

public class LoginViewModel :  BaseViewModel
{
    private readonly INutriRepository _repository;
    private string _username;
    private string _password;
    private string _errorMessage;

    public string Username { get => _username; set => SetProperty(ref _username, value); }
    public string Password { get => _password; set => SetProperty(ref _password, value); }
    public string ErrorMessage { get => _errorMessage; set => SetProperty(ref _errorMessage, value); }

    public ICommand LoginCommand { get; }
    public ICommand ShowRegisterCommand { get; }

    public LoginViewModel(INutriRepository repository)
    {
        _repository = repository;
        LoginCommand = new RelayCommand(async _ => await Login());
    }

    private async Task Login()
    {
        var user = await _repository.GetUserByUsernameAsync(Username);
        
        if (user != null && user.Password == Password)
        {
            OnLoginSuccess?.Invoke(user);
        }
        else
        {
            ErrorMessage = "Невірний логін або пароль";
        }
    }

    public event Action<User> OnLoginSuccess;
}