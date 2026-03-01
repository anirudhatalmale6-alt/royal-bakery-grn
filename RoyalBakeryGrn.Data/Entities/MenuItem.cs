using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoyalBakeryGrn.Data.Entities
{
    [Table("MenuItems")] // maps to your SQL table
    public class MenuItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(200)] // optional, adjust according to DB column
        public string Name { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public int MenuCategoryId { get; set; }
    }
}
