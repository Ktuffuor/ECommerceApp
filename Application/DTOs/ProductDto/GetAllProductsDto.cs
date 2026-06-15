namespace Application.DTOs.ProductDto;

public class GetAllProductsDto
{
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; } = string.Empty;
    public string? ProductDesc { get; set; } = string.Empty;
    public decimal ProductPrice { get; set; }
    public string? ProductPicUrl { get; set; } = string.Empty;
    public string? ProductBrand { get; set; } = string.Empty;
    public int ProductStockQty { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; } = false;
}