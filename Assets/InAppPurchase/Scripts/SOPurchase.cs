using System;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Core.Purchase
{
    [CreateAssetMenu(fileName = "ProjectStore", menuName = "ScriptableObjects/Store/ProjectStore")]
    public class SOPurchase : ScriptableObject,  IStoreListener
    {
        private IStoreController StoreController;
        private IExtensionProvider m_StoreExtensionProvider;
        private StoreProduct CurrentProduct;

        [SerializeField] NonConsumableProduct[] NonConsumableProducts;
        [SerializeField] ConsumableProduct[] ConsumableProducts;

        [System.Obsolete]
        public void InitializePurchasing()
        {

            if (IsInitialized())
            {
                return;
            }
            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
            foreach (NonConsumableProduct product in NonConsumableProducts)
            {
                builder.AddProduct(product.ProductID, ProductType.NonConsumable);
            }
            foreach (ConsumableProduct product in ConsumableProducts)
            {
                builder.AddProduct(product.ProductID, ProductType.Consumable);
            }

            UnityPurchasing.Initialize(this, builder);

        }

        public bool IsInitialized()
        {
            return StoreController != null && m_StoreExtensionProvider != null;
        }


        public string GetPrice(string productID)
        {
            if (IsInitialized())
            {
                return StoreController.products.WithID(productID).metadata.localizedPriceString;

            }
            else
            {
                return " ";
            }
        }

        public void BuyProduct(StoreProduct buyProduct)
        {
            CurrentProduct = buyProduct;
            Product product = StoreController.products.WithID(buyProduct.ProductID);

            if (product != null && product.availableToPurchase)
            {
                Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                StoreController.InitiatePurchase(product);
            }
            else
            {

                Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
            }
        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
        {
            if (args.purchasedProduct.definition.id == CurrentProduct.ProductID)
            {
                CurrentProduct.BuyProduct();
            }
            else
            {
                Debug.Log(string.Format("ProcessPurchase: FAIL. Unrecognized product: '{0}'", args.purchasedProduct.definition.id));
            }

            return PurchaseProcessingResult.Complete;
        }

        void IStoreListener.OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            //Debug.Log("OnInitialized: PASS");
            StoreController = controller;

            m_StoreExtensionProvider = extensions;
        }

        void IStoreListener.OnInitializeFailed(InitializationFailureReason error)
        {
            Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
        }
        void IStoreListener.OnInitializeFailed(InitializationFailureReason error, string message)
        {
            Debug.Log("OnInitializeFailed InitializationFailureReason:" + error);
        }
        void IStoreListener.OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
        {
            Debug.Log(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
        }
    }
}
