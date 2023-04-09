namespace KassaSys.Register
{
    public interface ICashRegister
    {
        int FetchReceiptNumber();

        void AddToReceipt(int productId, int amount);

        void SaveReceipt();
    }
}