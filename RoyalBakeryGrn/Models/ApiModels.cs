namespace RoyalBakeryGrn.Models
{
    // ===== Menu DTOs =====
    public class MenuItemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int MenuCategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public bool IsQuick { get; set; }
    }

    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    // ===== Stock DTOs =====
    public class StockDto
    {
        public int MenuItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    // ===== GRN DTOs =====
    public class GrnDto
    {
        public int Id { get; set; }
        public string GRNNumber { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<GrnItemDto> Items { get; set; } = new();
    }

    public class GrnItemDto
    {
        public int Id { get; set; }
        public int MenuItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public int CurrentQuantity { get; set; }

        /// <summary>Sold = Original Quantity - Current Remaining</summary>
        public int SoldQuantity => Quantity - CurrentQuantity;

        /// <summary>Display string for the UI</summary>
        public string SoldInfo => $"Sold: {SoldQuantity} | Remaining: {CurrentQuantity} (min qty: {SoldQuantity})";
    }

    public class CreateGrnRequest
    {
        public List<CreateGrnItemRequest> Items { get; set; } = new();
    }

    public class CreateGrnItemRequest
    {
        public int MenuItemId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }

    // ===== Adjustment DTOs =====
    public class AdjustmentDto
    {
        public int Id { get; set; }
        public int GRNId { get; set; }
        public string GRNNumber { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string AdminCode { get; set; } = string.Empty;
        public bool IsApproved { get; set; }
        public DateTime RequestedAt { get; set; }
        public List<AdjustmentItemDto> Items { get; set; } = new();
    }

    public class AdjustmentItemDto
    {
        public int MenuItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int RequestedQuantity { get; set; }
        public decimal Price { get; set; }
    }

    public class CreateAdjustmentRequest
    {
        public int GRNId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public List<AdjustmentItemDto> Items { get; set; } = new();
    }

    public class ApproveAdjustmentRequest
    {
        public string AdminCode { get; set; } = string.Empty;
    }

    // ===== Direct GRN Edit DTOs =====
    public class DirectEditGrnRequest
    {
        public string Reason { get; set; } = string.Empty;
        public List<AdjustmentItemDto> Items { get; set; } = new();
    }

    public class GrnEditLogDto
    {
        public int Id { get; set; }
        public int GRNId { get; set; }
        public string GRNNumber { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string ChangeSummary { get; set; } = string.Empty;
        public DateTime EditedAt { get; set; }
    }

    // ===== Clearance DTOs =====
    public class ClearanceDto
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public int MenuItemId { get; set; }
        public string ItemName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? Note { get; set; }
    }

    public class CreateClearanceRequest
    {
        public int MenuItemId { get; set; }
        public int Quantity { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string? Note { get; set; }
    }

    // ===== Generic API error =====
    public class ApiError
    {
        public string Message { get; set; } = string.Empty;
    }

    // ===== Auth DTOs =====
    public class LoginRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
