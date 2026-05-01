using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NutriTrack.Models;

public class FoodLog
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime ConsumptionDate { get; set; }

    public double ServingSizeGrams { get; set; }
    
    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public virtual User User { get; set; }
    
    public int ProductId { get; set; }
    [ForeignKey("ProductId")]
    public virtual Product Product { get; set; }
    
    [NotMapped]
    public double TotalCalories => (Product.Calories * ServingSizeGrams) / 100;
}