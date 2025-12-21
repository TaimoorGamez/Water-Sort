using TMPro;
using UnityEngine;
using Core.States;
using DG.Tweening;
using Core.Events;
using Core.Plugins;
using Core.Purchase;
using Core.GamePlay;
using Core.Plugins.Ads;
using Core.DB.Variables;
using System.Collections;
using Core.Plugins.Firebase;
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
        string _privacyLink = "https://sites.google.com/view/sortpaint-privacy-policy/", _adsManagerPath = "SDK/AdsManager",
               _loadingTxt = "Downloading...     ";
        AsyncOperationHandle<GameObject> _adsManagerhandle;
        Coroutine _downloadRotine;

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
            var checkHandle = Addressables.CheckForCatalogUpdates(false);
            yield return checkHandle;

            if (checkHandle.Status == AsyncOperationStatus.Succeeded && checkHandle.Result != null && checkHandle.Result.Count > 0)
            {
                var updateHandle = Addressables.UpdateCatalogs(checkHandle.Result, false);
                yield return updateHandle;
            }

            Addressables.Release(checkHandle);
        }

        IEnumerator DownloadRemoteLevels()
        {
            DownloadingScreen.SetActive(true);

            var sizeHandle = Addressables.GetDownloadSizeAsync("remote_lvl");
            yield return sizeHandle;

            long downloadSize = sizeHandle.Result;

            if (downloadSize > 0)
            {
                var downloadHandle =
                    Addressables.DownloadDependenciesAsync("remote_lvl", true);

                while (!downloadHandle.IsDone)
                {
                    float percent = downloadHandle.PercentComplete;
                    UpdateLoadingUI(percent);
                    yield return new WaitForSeconds(0.1f);
                }

                Addressables.Release(downloadHandle);
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

        private void OnDisable()
        {
            if(_downloadRotine != null)
            {
                StopCoroutine(_downloadRotine);
                _downloadRotine = null;
            }
        }
    }
}