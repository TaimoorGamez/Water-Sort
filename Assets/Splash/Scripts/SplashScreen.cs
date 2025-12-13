using TMPro;
using Core.Store;
using UnityEngine;
using DG.Tweening;
using Core.Events;
using Core.Plugins;
using Core.ToastMsg;
using Core.Variables;
using UnityEngine.UI;
using Core.DB.Variables;

namespace Core.Screen
{
    public class SplashScreen : UiScreens
    {
        [SerializeField] ToastManager ToastMsnger;
        [SerializeField] Initialization FirebaseInit;
        [SerializeField] ItemData DefaultCap, DefaultFlame, DefaultSpray;
        [SerializeField] DBInt LvlNum, FFT;
        [SerializeField] SOIntegerEvents ActiveStateEvent, DestroyStateEvent;
        [SerializeField] SOInterger MainMenuStateIndex, GamePlayStateIndex, MinLvlNum;
        [SerializeField] Transform FillImage;
        [SerializeField] TextMeshProUGUI LoadingText;
        [SerializeField] Image LogoImage;

        float _loadingTime = 2;
        string _loadingTxt = "Loading...     ";

        private void Start()
        {
            if (FFT.Value != 1)
            {
                DefaultCap.IsPurchased = true;
                DefaultFlame.IsPurchased = true;
                DefaultSpray.IsPurchased = true;
                FFT.Value = 1;
            }
            FirebaseInit.InitPlugin();
            LogoImage.DOFillAmount(1, _loadingTime).SetEase(Ease.Linear);
            FillImage.DOScaleX(1f, _loadingTime).SetEase(Ease.Linear).OnUpdate(() =>
            {
                float currentX = FillImage.localScale.x;
                int percent = (int)(currentX * 100f);
                LoadingText.text = _loadingTxt + percent + "%";
            }).OnComplete(() =>
            {
                ToastMsnger.InitToastMsg();
                if (LvlNum.Value <= MinLvlNum.Value)
                {
                    SimpleEventsHolder.InitLvlEvent?.Invoke();
                    ActiveStateEvent.InvokeSOEvent(GamePlayStateIndex.Value);
                }
                else
                {
                    ActiveStateEvent.InvokeSOEvent(MainMenuStateIndex.Value);
                }
                DestroyStateEvent.InvokeSOEvent(0);
            });
        }
    }
}
