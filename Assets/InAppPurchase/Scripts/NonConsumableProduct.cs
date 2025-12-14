using UnityEngine;
using Core.Events;
using Core.DB.Variables;

namespace Core.Purchase
{
    [CreateAssetMenu(fileName ="storeProduct", menuName = "ScriptableObjects/Store/nonConsumable")]
    public class NonConsumableProduct : StoreProduct
    {
        [SerializeField] string ProductName;

        public override void BuyProduct()
        {
            DBVariableDictionariesHolder.NonConsumableProductsData[ProductName].Value = 1;
            InvokeEventFromKey();
        }

        void InvokeEventFromKey()
        {
            if (string.IsNullOrEmpty(ProductName))
            {
                Debug.LogWarning($"{name}: EventKey is empty!");
                return;
            }

            if (EventDictionariesHolder.NonConsumableProductsEvents.TryGetValue(ProductName, out var evt))
            {
                evt?.Invoke();
            }
            else
            {
                Debug.LogWarning($"{name}: No event registered for key '{ProductName}'");
            }
        }
    }
}
