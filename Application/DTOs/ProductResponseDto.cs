namespace Application.DTOs;

public class ProductResponseDto
{
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; } = string.Empty;
    public string? ProductDesc { get; set; } = string.Empty;
    public decimal ProductPrice { get; set; }
    public int ProductStockQty { get; set; }
    public string? ProductBrand { get; set; }
}