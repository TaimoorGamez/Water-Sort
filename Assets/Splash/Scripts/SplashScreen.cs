using TMPro;
using Core.Store;
using UnityEngine;
using DG.Tweening;
using Core.Events;
using Core.States;
using Core.GamePlay;
using UnityEngine.UI;
using Core.DB.Variables;

namespace Core.Screen
{
    public class SplashScreen : UiScreens
    {
        [SerializeField] Transform FillImage;
        [SerializeField] TextMeshProUGUI LoadingText;
        [SerializeField] Image LogoImage;

        float _loadingTime = 2;
        string _loadingTxt = "Loading...     ";

        private void Start()
        {
            if (DBVariablesHolder.FFT.Value != 1)
            {
                StorageData.AllItems[StorageData.FlameThrowersKey][0].IsPurchased = true;
                StorageData.AllItems[StorageData.CapsKey][0].IsPurchased = true;
                StorageData.AllItems[StorageData.SpraysKey][0].IsPurchased = true;
                DBVariablesHolder.FFT.Value = 1;
            }
            LogoImage.DOFillAmount(1, _loadingTime).SetEase(Ease.Linear);
            FillImage.DOScaleX(1f, _loadingTime).SetEase(Ease.Linear).OnUpdate(() =>
            {
                float currentX = FillImage.localScale.x;
                int percent = (int)(currentX * 100f);
                LoadingText.text = _loadingTxt + percent + "%";
            }).OnComplete(() =>
            {
                if (DBVariablesHolder.LvlNum.Value <= LevelsManager.I.MinLvlCount)
                {
                    SimpleEventsHolder.InitLvlEvent?.Invoke();
                    StateManager.I.ActiveState(StateManager.I.GamePlayStatePath);
                }
                else
                {
                    StateManager.I.ActiveState(StateManager.I.MainMenuStatePath);
                }
                StateManager.I.DestroyState(StateManager.I.SplashStatePath);
            });
        }
    }
}
