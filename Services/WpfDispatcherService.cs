namespace NutriTrack.Services;

public class WpfDispatcherService : IDispatcherService
{
    public void Invoke(Action action)
    {
        App.Current.Dispatcher.Invoke(action);
    }
}