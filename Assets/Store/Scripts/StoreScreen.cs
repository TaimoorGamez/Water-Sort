using TMPro;
using Core.Events;
using DG.Tweening;
using UnityEngine;
using Core.DataStructure;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.Screen
{
    public class StoreScreen : UiScreens
    {
        [SerializeField] Transform FillImage;
        [SerializeField] TextMeshProUGUI LoadingText;
        [SerializeField] GameObject Loading;
        [SerializeField] string FlameItemsPath, SprayItemsPath, CapItemsPath;
        [SerializeField] int MaxFlameThrowers, MaxSprayCans, MaxCaps;

        bool _isClearing = false;
        string _loadingTxt = "Loading...     ";

        private void OnEnable()
        {
            LoadStoreItems();
        }

        private async void OnDisable()
        {
            await ClearEverythingAsync();
        }

        public override void OnOpen()
        {
            Loading.SetActive(false);
            Body.gameObject.SetActive(true);
            SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(3);
            Body.DOAnchorPosX(0, _transitionDuration).SetEase(Ease.OutBack);
        }

        async void LoadStoreItems()
        {
            Loading.SetActive(true);
            await ClearEverythingAsync();

            Dictionary<int, GameObject> tempDictionary = new Dictionary<int, GameObject>();

            UpdateLoadingUI(0);

            // total items count for progress
            int totalCount = MaxCaps + MaxFlameThrowers + MaxSprayCans;
            int loadedCount = 0;

            // ============================================================
            // --------------------- LOAD CAPS -----------------------------
            // ============================================================

            for (int i = 0; i < MaxCaps; i++)
            {
                string address = CapItemsPath + i;   // example: "Store/Cap/" + 0

                AsyncOperationHandle<GameObject> handle = Addressables.LoadAssetAsync<GameObject>(address);

                await handle.Task;

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    tempDictionary.Add(i, handle.Result);
                }

                // update progress
                loadedCount++;
                float progress = (float)loadedCount / (float)totalCount;
                UpdateLoadingUI(progress);
            }
            await Task.Yield();
            GlobalDataStructures.StoreItemsContainer.Add("Cap", tempDictionary);
            await Task.Yield();

            // ============================================================
            // ------------------- LOAD FLAME THROWERS ---------------------
            // ============================================================
            tempDictionary.Clear();

            for (int i = 0; i < MaxFlameThrowers; i++)
            {
                string address = FlameItemsPath + i;

                AsyncOperationHandle<GameObject> handle =
                    Addressables.LoadAssetAsync<GameObject>(address);

                await handle.Task;

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                        tempDictionary.Add(i, handle.Result);
                }

                loadedCount++;
                float progress = (float)loadedCount / (float)totalCount;
                UpdateLoadingUI(progress);
            }
            await Task.Yield();
            GlobalDataStructures.StoreItemsContainer.Add("FlameThrower", tempDictionary);
            await Task.Yield();

            // ============================================================
            // --------------------- LOAD SPRAY CANS -----------------------
            // ============================================================
            tempDictionary.Clear();

            for (int i = 0; i < MaxSprayCans; i++)
            {
                string address = SprayItemsPath + i;

                AsyncOperationHandle<GameObject> handle =
                    Addressables.LoadAssetAsync<GameObject>(address);

                await handle.Task;

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    tempDictionary.Add(i, handle.Result);
                }

                loadedCount++;
                float progress = (float)loadedCount / (float)totalCount;
                UpdateLoadingUI(progress);
            }
            await Task.Yield();
            GlobalDataStructures.StoreItemsContainer.Add("Spray", tempDictionary);
            await Task.Yield();

            // ============================================================
            // ------------------------- DONE ------------------------------
            // ============================================================

            FillImage.DOScaleX(1f, 0.1f);
            OnOpen();
        }
        
        void UpdateLoadingUI(float progress)
        {
            FillImage.DOScaleX(progress, 0.1f).SetEase(Ease.Linear).SetUpdate(true);
            int percent = (int)(progress * 100f);
            LoadingText.text = _loadingTxt + percent.ToString() + "%";
        }

        public override void OnClose()
        {
            Body.DOKill();
            SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(2);
            Body.DOAnchorPosX(1500, _transitionDuration / 2).SetEase(Ease.InBack).OnComplete(() => {
                Body.gameObject.SetActive(false);
                gameObject.SetActive(false);
                });
        }

        async Task ClearEverythingAsync()
        {
            if (_isClearing) return;
            _isClearing = true;

            await ClearDictionariesAsync();
            await Task.Yield();

            System.GC.Collect();

            try
            {
                Caching.ClearCache();
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.ToString());
            }

            _isClearing = false;
        }
        async Task ClearDictionariesAsync()
        {
            foreach (var outerPair in GlobalDataStructures.StoreItemsContainer)
            {
                var dict = outerPair.Value;

                foreach (var handlePair in dict)
                {
                    var handle = handlePair.Value;

                    if (handle != null)
                        Addressables.Release(handle);
                }

                dict.Clear();
                await Task.Yield();
            }

            GlobalDataStructures.StoreItemsContainer.Clear();
            await Task.Yield();
        }

    }
}
