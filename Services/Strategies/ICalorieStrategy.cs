using NutriTrack.Models;

namespace NutriTrack.Services.Strategies;

public interface ICalorieStrategy
{
    double CalculateBMR(User user);
}