using KassaSys.Enum;

namespace KassaSys.Register;

public class CashRegisterList
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double Price { get; set; }
    public int Count { get; set; }
    public ProductType Type { get; set; }
    public string Discount { get; set; }
}