using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoyalBakeryGrn.Data.Entities
{
    public class GRNAdjustmentRequest
    {
        public int Id { get; set; }
        public int GRNId { get; set; }
        public GRN GRN { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string AdminCode { get; set; } = string.Empty;
        public bool IsApproved { get; set; } = false;
        public DateTime RequestedAt { get; set; } = DateTime.Now;

        // New: store all requested changes
        public ICollection<GRNAdjustmentRequestItem> RequestedItems { get; set; } = new List<GRNAdjustmentRequestItem>();
    }


}
