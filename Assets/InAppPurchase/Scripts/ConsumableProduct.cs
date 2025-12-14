using UnityEngine;
using Core.Economy;

namespace Core.Purchase
{
    [CreateAssetMenu(fileName = "storeProduct", menuName = "ScriptableObjects/Store/Consumable")]
    public class ConsumableProduct : StoreProduct
    {
        [SerializeField] protected int Amount;
        [SerializeField] protected string CurrencyName;

        public override void BuyProduct()
        {
            CurrencyDictionariesHolder.AllCurrencies[CurrencyName].Amount += Amount;
        }
    }
}
