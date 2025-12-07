using UnityEngine;

namespace Core.Purchase
{
    public class StoreProduct : ScriptableObject, IStoreProduct
    {
       [SerializeField] string _productId;

        public string ProductID
        {
            get
            {
                if (!string.IsNullOrEmpty(_productId))
                {
                    return _productId;
                }
                else
                {
                    Debug.LogError("ProductID cannot be an empty string.");
                    return " ";
                }
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    _productId = value;
                }
                else
                {
                    Debug.LogError("ProductID cannot be an empty string.");
                }
            }
        }

        public virtual void BuyProduct()
        {

        }
    }
}
