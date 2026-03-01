
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoyalBakeryGrn.Data.Entities
{
    public class Stock
    {
        public int Id { get; set; }

        public int MenuItemId { get; set; }

        [Required]
        public string MenuItem { get; set; } = string.Empty; // ✅ default initialized

        public int Quantity { get; set; }
    }
}
