using UnityEngine;
using Core.Events;
using Core.DB.Variables;

namespace Core.Purchase
{
    [CreateAssetMenu(fileName ="storeProduct", menuName = "ScriptableObjects/Store/nonConsumable")]
    public class NonConsumableProduct : StoreProduct
    {
        [SerializeField] DBInt ProductDB;
        [SerializeField] string EventKey;

        public override void BuyProduct()
        {
            if (ProductDB.Value != 1)
            {
                ProductDB.Value = 1;
                InvokeEventFromKey();
            }
        }

        void InvokeEventFromKey()
        {
            if (string.IsNullOrEmpty(EventKey))
            {
                Debug.LogWarning($"{name}: EventKey is empty!");
                return;
            }

            if (EventDictionariesHolder.NonConsumableProductsEvents.TryGetValue(EventKey, out var evt))
            {
                evt?.Invoke();
            }
            else
            {
                Debug.LogWarning($"{name}: No event registered for key '{EventKey}'");
            }
        }
    }
}
