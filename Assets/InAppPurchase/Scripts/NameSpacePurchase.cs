namespace Core.Purchase
{
    public interface IStoreProduct
    {
        string ProductID { get; set; }

        void BuyProduct();
    }
}

