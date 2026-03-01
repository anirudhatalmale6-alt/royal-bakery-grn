using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoyalBakeryGrn.Data.Entities
{
    public class GRNAdjustmentRequestItem
    {
        public int Id { get; set; }
        public int GRNAdjustmentRequestId { get; set; }
        public GRNAdjustmentRequest GRNAdjustmentRequest { get; set; }

        public int MenuItemId { get; set; }       // The menu item to adjust
        public string ItemName { get; set; } = string.Empty; // Optional: store name for reference
        public int RequestedQuantity { get; set; }           // Quantity requested
        public decimal Price { get; set; }                   // Optional: store price for reference
    }
}
