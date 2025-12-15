using System;
using UnityEngine;
using Core.Events;
using Core.Economy;
using Core.DB.Variables;

namespace Core.Purchase
{

    [Serializable]
    public class StoreProduct
    {
        public string ProductID;

        public virtual void BuyProduct()
        {

        }
    }

    [Serializable]
    public class NonConsumableProduct : StoreProduct
    {
        public override void BuyProduct()
        {
            DBVariableDictionariesHolder.NonConsumableProductsData[ProductID].Value = 1;
            InvokeEventFromKey();
        }

        void InvokeEventFromKey()
        {
            if (EventDictionariesHolder.NonConsumableProductsEvents.TryGetValue(ProductID, out var evt))
            {
                evt?.Invoke();
            }
            else
            {
                Debug.LogWarning($"{ProductID}: No event registered for key");
            }
        }
    }

    [Serializable]
    public class ConsumableProduct : StoreProduct
    {
        public int Amount;
        public string CurrencyName;

        public override void BuyProduct()
        {
            CurrencyDictionariesHolder.AllCurrencies[CurrencyName].Amount += Amount;
        }
    }
}

