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
    
        var loginWindow = new LoginWindow();
        var loginVM = new LoginViewModel(repo);
    
        loginWindow.DataContext = loginVM;

        loginVM.OnLoginSuccess += (user) => 
        {
            var mainWindow = new MainWindow(user, repo);
            mainWindow.Show();
            loginWindow.Close();
        };

        loginWindow.Show();
    }
}