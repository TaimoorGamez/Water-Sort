using UnityEngine;
using Core.Events;
using Core.DB.Variables;

namespace Core.Purchase
{
    [CreateAssetMenu(fileName ="storeProduct", menuName = "ScriptableObjects/Store/nonConsumable")]
    public class NonConsumableProduct : StoreProduct
    {
        [SerializeField] DBInt ProductDB;
        [SerializeField] SOEvents ProductEvent;

        public override void BuyProduct()
        {
            if (ProductDB.Value != 1)
            {
                ProductDB.Value = (1);
                ProductEvent.InvokeSOEvent();
            }
        }
    }
}
