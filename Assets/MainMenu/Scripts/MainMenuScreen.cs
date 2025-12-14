using TMPro;
using UnityEngine;
using Core.Events;
using DG.Tweening;
using Core.States;
using Core.Purchase;
using Core.Plugins.Ads;
using Core.DB.Variables;
using Core.Plugins.Firebase;
using Core.GamePlay.WaterSort;

namespace Core.Screen
{
    public class MainMenuScreen : UiScreens
    {
        [SerializeField] DBInt LvlNum;
        [SerializeField] FirebaseInitialization FirebaseInit;
        [SerializeField] InAppPurchase InAppPurchaser;
        [SerializeField] TextMeshProUGUI[] Lvls;
        [SerializeField] Transform LevelView;
        [SerializeField] RectTransform LevelsHolder;

        int _activeLvl = 3;
        string _privacyLink = "https://sites.google.com/view/sortpaint-privacy-policy/";

        private void Start()
        {
            LevelsManager.I.TempLvlIndex = -1;
            if(FirebaseInit.IsFirebaseInit)
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
            SingleIntegerEventsHolder.ActiveStateEvent?.Invoke(StateManager.I.GamePlayStateIndex);
            SingleIntegerEventsHolder.DestroyStatEvent?.Invoke(StateManager.I.MainMenuStateIndex);
        }

        public void OpenPrivacyPolicy()
        {
            Application.OpenURL(_privacyLink);
        }

        private void InitializeAds()
        {
            //if(!AdsManager.I.IsInitialized)
            //    AdsManager.I.InitPlugin();
        }
    }
}
