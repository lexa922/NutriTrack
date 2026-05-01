using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using NutriTrack.Data;
using NutriTrack.Data.Repositories;
using NutriTrack.ViewModels;

namespace NutriTrack;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        var context = new AppDbContext();
        var repository = new SqliteNutriRepository(context);
        
        DataContext = new MainViewModel(repository);
    }
}