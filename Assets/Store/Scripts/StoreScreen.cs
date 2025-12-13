using TMPro;
using Core.Events;
using DG.Tweening;
using UnityEngine;
using Core.Variables;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.Screen
{
    public class StoreScreen : UiScreens
    {
        [SerializeField] SOAsyncGameObjectIList StoreItemsList;
        [SerializeField] SODictionary_Int_Gameobject CapItemsDictionary, FlameItemsDictionary, SprayItemsDictionary;
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

            // ensure dictionaries exist
            if (CapItemsDictionary.DictionaryValue == null)
                CapItemsDictionary.DictionaryValue = new Dictionary<int, GameObject>();

            if (FlameItemsDictionary.DictionaryValue == null)
                FlameItemsDictionary.DictionaryValue = new Dictionary<int, GameObject>();

            if (SprayItemsDictionary.DictionaryValue == null)
                SprayItemsDictionary.DictionaryValue = new Dictionary<int, GameObject>();

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

                AsyncOperationHandle<GameObject> handle =
                    Addressables.LoadAssetAsync<GameObject>(address);

                await handle.Task;

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                        CapItemsDictionary.DictionaryValue.Add(i, handle.Result);
                }

                // update progress
                loadedCount++;
                float progress = (float)loadedCount / (float)totalCount;
                UpdateLoadingUI(progress);
            }

            // ============================================================
            // ------------------- LOAD FLAME THROWERS ---------------------
            // ============================================================

            for (int i = 0; i < MaxFlameThrowers; i++)
            {
                string address = FlameItemsPath + i;

                AsyncOperationHandle<GameObject> handle =
                    Addressables.LoadAssetAsync<GameObject>(address);

                await handle.Task;

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                        FlameItemsDictionary.DictionaryValue.Add(i, handle.Result);
                }

                loadedCount++;
                float progress = (float)loadedCount / (float)totalCount;
                UpdateLoadingUI(progress);
            }

            // ============================================================
            // --------------------- LOAD SPRAY CANS -----------------------
            // ============================================================

            for (int i = 0; i < MaxSprayCans; i++)
            {
                string address = SprayItemsPath + i;

                AsyncOperationHandle<GameObject> handle =
                    Addressables.LoadAssetAsync<GameObject>(address);

                await handle.Task;

                if (handle.Status == AsyncOperationStatus.Succeeded)
                {
                    SprayItemsDictionary.DictionaryValue.Add(i, handle.Result);
                }

                loadedCount++;
                float progress = (float)loadedCount / (float)totalCount;
                UpdateLoadingUI(progress);
            }

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

        async Task ClearEverythingAsync(bool clearDiskCache = false)
        {
            if (_isClearing) return;
            _isClearing = true;

            // 1) Stop any tweens on UI elements (progress bar and body)
            try
            {
                if (FillImage != null)
                {
                    DOTween.Kill(FillImage, complete: false);
                }

                if (Body != null)
                {
                    DOTween.Kill(Body, complete: false);
                }
            }
            catch { /* ignore */ }

            // 2) Release Addressables load handle for the list
            try
            {
                if (StoreItemsList != null && StoreItemsList.ListValue.IsValid())
                {
                    Addressables.Release(StoreItemsList.ListValue);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogWarning($"StoreScreen: Addressables.Release failed: {e.Message}");
            }

            // set to default to avoid later accidental use
            if (StoreItemsList != null)
                StoreItemsList.ListValue = default;

            // 3) Destroy all GameObjects stored inside dictionaries and clear them
            await ClearDictionaryAsync(CapItemsDictionary);
            await ClearDictionaryAsync(FlameItemsDictionary);
            await ClearDictionaryAsync(SprayItemsDictionary);

            // 5) Wait one frame so Unity processes Destroy()
            await Task.Yield();

            // 6) Force unload unused assets and GC collect to free memory
            AsyncOperation unloadOp = Resources.UnloadUnusedAssets();
            while (!unloadOp.isDone)
                await Task.Yield();

            System.GC.Collect();

            // 7) Optionally clear disk cache (not necessary for local-only store; use with caution)
            if (clearDiskCache)
            {
                try
                {
                    bool cleared = Caching.ClearCache();
                    //Debug.Log("StoreScreen: Caching.ClearCache() returned " + cleared);
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning("StoreScreen: Caching.ClearCache() exception: " + e.Message);
                }
            }

            _isClearing = false;
        }

        async Task ClearDictionaryAsync(SODictionary_Int_Gameobject dictionary)
        {
            if (dictionary.DictionaryValue == null) return;
            foreach (var kvp in dictionary.DictionaryValue)
            {
                GameObject obj = kvp.Value;
                if (obj != null)
                {
                    Addressables.Release(obj);
                }
            }
            dictionary.DictionaryValue.Clear();
            await Task.Yield();
        }

    }
}
