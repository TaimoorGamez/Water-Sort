using TMPro;
using UnityEngine;

namespace Core.Purchase
{
    public class ProductHandler : MonoBehaviour
    {
        [SerializeField] SOPurchase SoStore;
        [SerializeField] StoreProduct CurrentProdut;
        [SerializeField] TextMeshProUGUI priceText;


        private void Start()
        {
            UpdatePriceValue();
        }
        
        private void UpdatePriceValue()
        {
            string currentPrice = SoStore.GetPrice(CurrentProdut.ProductID);
            if (currentPrice != " ")
            {
                priceText.text = currentPrice;
            }
        }

        public void OnClickBuyButton()
        {
            SoStore.BuyProduct(CurrentProdut);
        }
    }
}
