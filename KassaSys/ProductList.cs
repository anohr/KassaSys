using KassaSys.Enum;

namespace KassaSys;

public class ProductList
{
	public int Id { get; set; }
	public string Name { get; set; }
	public double Price { get; set; }
	public ProductType Type { get; set; }
}
