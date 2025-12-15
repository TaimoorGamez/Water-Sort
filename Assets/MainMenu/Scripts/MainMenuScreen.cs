using TMPro;
using Core.Events;
using UnityEngine;
using Core.States;
using DG.Tweening;
using Core.Plugins;
using Core.GamePlay;
using Core.Purchase;
using Core.Plugins.Ads;
using Core.DB.Variables;
using Core.Plugins.Firebase;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.Screen
{
    public class MainMenuScreen : UiScreens
    {
        [SerializeField] InAppPurchase InAppPurchaser;
        [SerializeField] TextMeshProUGUI[] Lvls;
        [SerializeField] Transform LevelView;
        [SerializeField] RectTransform LevelsHolder;

        int _activeLvl = 3;
        string _privacyLink = "https://sites.google.com/view/sortpaint-privacy-policy/", _adsManagerPath = "SDK/AdsManager";
        AsyncOperationHandle<GameObject> _adsManagerhandle;

        private void Start()
        {
            if (FirebaseHandler.I != null)
            {
                if (FirebaseHandler.I.IsInitialize && RemoteDataHolder.IsInternetWorking)
                {
                    if (!InAppPurchaser.IsInitialized)
                    {
                        InAppPurchaser.InitializePurchasing();
                        Invoke(nameof(InitializeAds), 1f);
                    }
                    else
                    {
                        InitializeAds();
                    }
                }
                else
                {
                    FirebaseHandler.I.InitPlugin();
                }
            }

            LevelsManager.I.TempLvlIndex = -1;
            for (int l = 0; l < Lvls.Length; l++)
            {
                if (l < _activeLvl)
                {
                    Lvls[l].text = (DBVariablesHolder.LvlNum.Value - (_activeLvl - l)).ToString();
                }
                else if (l > _activeLvl)
                {
                    Lvls[l].text = (DBVariablesHolder.LvlNum.Value + (l - _activeLvl)).ToString();
                }
                else
                {
                    Lvls[l].text = DBVariablesHolder.LvlNum.Value.ToString();
                }
            }

            LevelView.DOScale(1, 1).SetEase(Ease.OutBack).OnComplete(() => LevelsHolder.DOAnchorPosY(150, 1).SetEase(Ease.OutBack));
        }

        public void OnClickPlayButton()
        {
            SimpleEventsHolder.InitLvlEvent?.Invoke();
            StateManager.I.ActiveState(StateManager.I.GamePlayStatePath);
            StateManager.I.DestroyState(StateManager.I.MainMenuStatePath);
        }

        public void OpenPrivacyPolicy()
        {
            Application.OpenURL(_privacyLink);
        }

        private void InitializeAds()
        {
            if (AdsManager.I == null)
            {
                LoadAdsManager();
            }
            else if (!AdsManager.I.IsInitialized)
            {
                AdsManager.I.InitPlugin();
            }
        }

        async void LoadAdsManager()
        {
            _adsManagerhandle = Addressables.LoadAssetAsync<GameObject>(_adsManagerPath);

            await _adsManagerhandle.Task;

            if (_adsManagerhandle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject adsObj = Instantiate(_adsManagerhandle.Result);
                adsObj.name = "AdsManager";

                // Give 1 frame so Awake() runs & singleton is assigned
                await Task.Delay(1000);

                // Init ads
                if (AdsManager.I != null)
                {
                    AdsManager.I.InitPlugin();
                }
            }
            else
            {
                Debug.Log("Failed to load AdsManager from Addressables");
                Addressables.Release(_adsManagerhandle);
            }
        }

        private async void OnDisable()
        {
            await ReleaseHandler();
        }

        async Task ReleaseHandler()
        {
            if (!_adsManagerhandle.IsValid())
                return;

            Addressables.Release(_adsManagerhandle);
            while (_adsManagerhandle.IsValid())
                await Task.Yield();

            await Task.Yield();
        }
    }
}
