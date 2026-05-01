using NutriTrack.Models;
using NutriTrack.Services.Strategies;

namespace NutriTrack.Services;

public class CalorieCalculator
{
    private ICalorieStrategy _strategy;
    
    public void SetStrategy(ICalorieStrategy strategy) => _strategy = strategy;

    public double CalculateDailyGoal(User user)
    {
        if (_strategy == null) throw new InvalidOperationException("Strategy not set!");

        double bmr = _strategy.CalculateBMR(user);
        
        double multiplier = user.Activity switch
        {
            ActivityLevel.Sedentary => 1.2,
            ActivityLevel.Light => 1.375,
            ActivityLevel.Moderate => 1.55,
            ActivityLevel.Heavy => 1.725,
            ActivityLevel.Athlete => 1.9,
            _ => 1.2
        };

        return bmr * multiplier;
    }
}