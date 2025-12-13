using TMPro;
using UnityEngine;
using Core.Events;
using DG.Tweening;
using Core.Plugins;
using Core.Purchase;
using Core.Variables;
using Core.Plugins.Ads;
using Core.DB.Variables;

namespace Core.Screen
{
    public class MainMenuScreen : UiScreens
    {
        [SerializeField] DBInt LvlNum;
        [SerializeField] Initialization FirebaseInit;
        [SerializeField] AdmobInitialization AdmobInit;
        [SerializeField] InAppPurchase InAppPurchaser;
        [SerializeField] SOInterger MainMenuStateIndex, GamePlayStateIndex, TempLvlIndex, IsFirebaseInit, InAppInitialized, AdmobInitialized;
        [SerializeField] SOIntegerEvents ActiveStateEvent, DestroyStateEvent;
        [SerializeField] TextMeshProUGUI[] Lvls;
        [SerializeField] Transform LevelView;
        [SerializeField] RectTransform LevelsHolder;

        int _activeLvl = 3;
        string _privacyLink = "https://sites.google.com/view/sortpaint-privacy-policy/";

        private void Start()
        {
            TempLvlIndex.Value = -1;
            if(IsFirebaseInit.Value == 1)
            {
                if (InAppInitialized.Value == 0)
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
                FirebaseInit.InitPlugin();
            }

            for (int l = 0; l < Lvls.Length; l++)
            {
                if (l < _activeLvl)
                {
                    Lvls[l].text = (LvlNum.Value - (_activeLvl - l)).ToString();
                }
                else if (l > _activeLvl)
                {
                    Lvls[l].text = (LvlNum.Value + (l- _activeLvl)).ToString();
                }
                else
                {
                    Lvls[l].text = LvlNum.Value.ToString();
                }
            }

            LevelView.DOScale(1, 1).SetEase(Ease.OutBack).OnComplete(()=> LevelsHolder.DOAnchorPosY(150,1).SetEase(Ease.OutBack));
        }

        public void OnClickPlayButton()
        {
            SimpleEventsHolder.InitLvlEvent?.Invoke();
            ActiveStateEvent.InvokeSOEvent(GamePlayStateIndex.Value);
            DestroyStateEvent.InvokeSOEvent(MainMenuStateIndex.Value);
        }

        public void OpenPrivacyPolicy()
        {
            Application.OpenURL(_privacyLink);
        }

        private void InitializeAds()
        {
            if(AdmobInitialized.Value == 0)
                AdmobInit.InitPlugin();
        }
    }
}
