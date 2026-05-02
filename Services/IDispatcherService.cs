namespace NutriTrack.Services;

public interface IDispatcherService
{
    void Invoke(Action action);
}