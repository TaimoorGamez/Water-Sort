using TMPro;
using Core.Store;
using UnityEngine;
using DG.Tweening;
using Core.Events;
using Core.States;
using Core.Plugins;
using UnityEngine.UI;
using Core.DB.Variables;
using Core.GamePlay.WaterSort;

namespace Core.Screen
{
    public class SplashScreen : UiScreens
    {
        [SerializeField] Initialization FirebaseInit;
        [SerializeField] ItemData DefaultCap, DefaultFlame, DefaultSpray;
        [SerializeField] Transform FillImage;
        [SerializeField] TextMeshProUGUI LoadingText;
        [SerializeField] Image LogoImage;

        float _loadingTime = 2;
        string _loadingTxt = "Loading...     ";

        private void Start()
        {
            if (DBIntsHolder.FFT.Value != 1)
            {
                DefaultCap.IsPurchased = true;
                DefaultFlame.IsPurchased = true;
                DefaultSpray.IsPurchased = true;
                DBIntsHolder.FFT.Value = 1;
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
                if (DBIntsHolder.LvlNum.Value <= LevelsManager.I.MinLvlCount)
                {
                    SimpleEventsHolder.InitLvlEvent?.Invoke();
                    SingleIntegerEventsHolder.ActiveStateEvent?.Invoke(StateManager.I.GamePlayStateIndex);
                }
                else
                {
                    SingleIntegerEventsHolder.ActiveStateEvent?.Invoke(StateManager.I.MainMenuStateIndex);
                }
                SingleIntegerEventsHolder.DestroyStatEvent?.Invoke(0);
            });
        }
    }
}
