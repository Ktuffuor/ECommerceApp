namespace Application.DTOs.ProductDto;

public class GetAllProductsDto
{
    public List<ProductDto> Products { get; set; } = new();
}

public class ProductDto
{
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public string? ProductDesc { get; set; }
    public decimal ProductPrice { get; set; }
    public string? ProductPicUrl { get; set; }
    public string? ProductBrand { get; set; }
    public int ProductStockQty { get; set; }
    public DateTime CreatedAt { get; set; }
}