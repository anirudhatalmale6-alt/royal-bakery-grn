using RoyalBakeryGrn.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class GRNItem
{
    [Key]
    public int Id { get; set; }

    // Foreign Key to GRN
    public int GRNId { get; set; }
    public GRN GRN { get; set; } = null!;

    // Foreign Key to MenuItem
    [Required]
    public int MenuItemId { get; set; }  // store only ID in DB

    // Navigation property
    public MenuItem MenuItem { get; set; }  // optional navigation, no [Required]

    [Required]
    public int Quantity { get; set; }

    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }

    // New column for current stock/quantity
    [Required]
    public int CurrentQuantity { get; set; }
}
