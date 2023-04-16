namespace KassaSys.Register
{
    public interface ICashRegister
    {
        void AddToReceipt(int productId, int amount);

        void SaveReceipt();
    }
}