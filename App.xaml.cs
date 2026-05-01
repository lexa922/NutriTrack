using System.Configuration;
using System.Data;
using System.Windows;
using NutriTrack.Data;
using NutriTrack.Data.Repositories;
using NutriTrack.ViewModels;
using NutriTrack.Views;

namespace NutriTrack;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
    
        var context = new AppDbContext();
        context.Database.EnsureCreated();
        var repo = new SqliteNutriRepository(context);

        ShowLogin(repo);
    }

    private void ShowLogin(SqliteNutriRepository repo)
    {
        var loginWindow = new LoginWindow();
        var loginVM = new LoginViewModel(repo);
        loginWindow.DataContext = loginVM;
        
        loginVM.OnLoginSuccess += (user) => {
            var mainWin = new MainWindow(user, repo);
            mainWin.Show();
            loginWindow.Close();
        };

        loginVM.OnShowRegister += () => {
            ShowRegister(repo);
            loginWindow.Close();
        };

        loginWindow.Show();
    }

    private void ShowRegister(SqliteNutriRepository repo)
    {
        var regWindow = new RegisterWindow();
        var regVM = new RegisterViewModel(repo);
        regWindow.DataContext = regVM;

        regVM.OnLoginSuccess += (user) => {
            var mainWin = new MainWindow(user, repo);
            mainWin.Show();
            regWindow.Close();
        };

        regVM.OnBackToLogin += () => {
            ShowLogin(repo);
            regWindow.Close();
        };

        regWindow.Show();
    }
}