using UnityEngine;
using Core.Economy;

namespace Core.Purchase
{
    [CreateAssetMenu(fileName = "storeProduct", menuName = "ScriptableObjects/Store/Consumable")]
    public class ConsumableProduct : StoreProduct
    {
        [SerializeField] protected Currency CurrentCurency;
        [SerializeField] protected int Amount;

        public override void BuyProduct()
        {
            CurrentCurency.Amount += Amount;
        }
    }
}
