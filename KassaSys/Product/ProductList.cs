using KassaSys.Enum;

namespace KassaSys.Product;

public class ProductList
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public ProductType Type { get; set; }
}
