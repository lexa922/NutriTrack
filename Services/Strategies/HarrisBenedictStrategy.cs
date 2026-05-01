using NutriTrack.Models;

namespace NutriTrack.Services.Strategies;

public class HarrisBenedictStrategy : ICalorieStrategy
{
    public double CalculateBMR(User user)
    {
        if (user.UserGender == Gender.Male)
            return 88.36 + (13.4 * user.Weight) + (4.8 * user.Height) - (5.7 * user.Age);
        
        return 447.59 + (9.2 * user.Weight) + (3.1 * user.Height) - (4.3 * user.Age);
    }
}