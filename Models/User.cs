using System.ComponentModel.DataAnnotations;

namespace NutriTrack.Models;

public enum Gender { Male, Female }
public enum ActivityLevel { Sedentary, Light, Moderate, Heavy, Athlete }
public class User
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Username { get; set; }

    [Required]
    public string Password { get; set; }
    
    public string Name { get; set; }
    public int Age { get; set; }
    public double Height { get; set; }
    public double Weight { get; set; }
    public Gender UserGender { get; set; }
    public ActivityLevel Activity { get; set; }

    public virtual ICollection<FoodLog> Logs { get; set; } = new List<FoodLog>();
}