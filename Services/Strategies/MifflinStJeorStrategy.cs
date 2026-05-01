using NutriTrack.Models;

namespace NutriTrack.Services.Strategies;

public class MifflinStJeorStrategy : ICalorieStrategy
{
    public double CalculateBMR(User user)
    {
        double bmr = (10 * user.Weight) + (6.25 * user.Height) - (5 * user.Age);
        return user.UserGender == Gender.Male ? bmr + 5 : bmr - 161;
    }
}