using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoyalBakeryGrn.Data.Entities
{
    public class GRN
    {
        public int Id { get; set; }
        public string GRNNumber { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public List<GRNItem> Items { get; set; }
    }

}
