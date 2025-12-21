using Core.DB.Variables;
using Core.Events;
using Core.GamePlay;
using Core.Plugins;
using Core.Plugins.Ads;
using Core.Plugins.Firebase;
using Core.Purchase;
using Core.States;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.Screen
{
    public class MainMenuScreen : UiScreens
    {
        [SerializeField] InAppPurchase InAppPurchaser;
        [SerializeField] TextMeshProUGUI[] Lvls;
        [SerializeField] TextMeshProUGUI LoadingText;
        [SerializeField] Transform LevelView;
        [SerializeField] RectTransform LevelsHolder, FillImage;
        [SerializeField] GameObject DownloadingScreen, FeedbackBtn;

        int _activeLvl = 3, requiredFeedbackLvl = 15;
        string _privacyLink = "https://sites.google.com/view/sortpaint-privacy-policy/", _loadingTxt = "Downloading...     ";
        Coroutine _downloadRotine;
        AsyncOperationHandle<List<string>> _checkCatalogHandle;
        AsyncOperationHandle _updateCatalogHandle;
        AsyncOperationHandle<long> _sizeHandle;
        AsyncOperationHandle _downloadHandle;
        bool _releaseInProgress = false;

        private void Start()
        {
            if (FirebaseHandler.I != null)
            {
                if (FirebaseHandler.I.IsInitialize && RemoteDataHolder.IsInternetWorking)
                {
                    if(RemoteDataHolder.MaxLevelsAvailable > DBVariablesHolder.MaxLvlCount.Value)
                    {
                        DownloadingScreen.SetActive(true);
                        _downloadRotine = StartCoroutine(RemoteDownloadFlow());
                    }
                    else
                    {
                        InitializeSdkAdapters();
                    }
                }
                else
                {
                    FirebaseHandler.I.InitPlugin();
                }
            }

            if(DBVariablesHolder.LvlNum.Value > requiredFeedbackLvl)
            {
                FeedbackBtn.SetActive(true);
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

        void InitializeSdkAdapters()
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

        IEnumerator RemoteDownloadFlow()
        {
            yield return CheckAndUpdateCatalog();
            yield return DownloadRemoteLevels();
            DBVariablesHolder.MaxLvlCount.Value = RemoteDataHolder.MaxLevelsAvailable;
            DownloadingScreen.SetActive(false);
            InitializeSdkAdapters();
        }
        IEnumerator CheckAndUpdateCatalog()
        {
            _checkCatalogHandle = Addressables.CheckForCatalogUpdates(false);
            yield return _checkCatalogHandle;

            if (_checkCatalogHandle.Status == AsyncOperationStatus.Succeeded &&
                _checkCatalogHandle.Result != null &&
                _checkCatalogHandle.Result.Count > 0)
            {
                _updateCatalogHandle =
                    Addressables.UpdateCatalogs(_checkCatalogHandle.Result, false);

                yield return _updateCatalogHandle;
            }
        }

        IEnumerator DownloadRemoteLevels()
        {
            _sizeHandle = Addressables.GetDownloadSizeAsync("remote_lvl");
            yield return _sizeHandle;

            long downloadSize = _sizeHandle.Result;

            if (downloadSize > 0)
            {
                _downloadHandle =
                    Addressables.DownloadDependenciesAsync("remote_lvl", true);

                while (!_downloadHandle.IsDone)
                {
                    UpdateLoadingUI(_downloadHandle.PercentComplete);
                    yield return null;
                }
            }

            UpdateLoadingUI(1);
        }

        void UpdateLoadingUI(float progress)
        {
            FillImage.DOScaleX(progress, 0.1f).SetEase(Ease.Linear).SetUpdate(true);
            int txtPercent = (int)(progress * 100f);
            LoadingText.text = _loadingTxt + txtPercent.ToString() + "%";
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
            AdsManager.I?.InitPlugin();
        }

        private async void OnDisable()
        {
            if(_downloadRotine != null)
            {
                StopCoroutine(_downloadRotine);
                _downloadRotine = null;
            }
            await ReleaseAllAddressableHandlesSafelyAsync();
        }

        async Task ReleaseAllAddressableHandlesSafelyAsync()
        {
            if (_releaseInProgress)
                return;

            _releaseInProgress = true;

            await Task.Yield();


            if (_downloadHandle.IsValid())
            {
                Addressables.Release(_downloadHandle);
                _downloadHandle = default;
            }
            if (_sizeHandle.IsValid())
            {
                Addressables.Release(_sizeHandle);
                _sizeHandle = default;
            }
            if (_updateCatalogHandle.IsValid())
            {
                Addressables.Release(_updateCatalogHandle);
                _updateCatalogHandle = default;
            }
            if (_checkCatalogHandle.IsValid())
            {
                Addressables.Release(_checkCatalogHandle);
                _checkCatalogHandle = default;
            }
            await Task.Yield();

            _releaseInProgress = false;
        }

    }
}