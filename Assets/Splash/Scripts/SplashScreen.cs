using UnityEngine;
using DG.Tweening;
using Core.Events;
using Core.Plugins;
using Core.GamePlay;
using Core.Variables;
using Core.DB.Variables;

namespace Core.Screen
{
    public class SplashScreen : MonoBehaviour
    {
        [SerializeField] Initialization FirebaseInit;
        [SerializeField] ItemData DefaultCap, DefaultFlame, DefaultSpray;
        [SerializeField] SOEvents InitLevelEvent;
        [SerializeField] DBInt LvlNum, FFT;
        [SerializeField] SOIntegerEvents ActiveStateEvent, DestroyStateEvent;
        [SerializeField] LevelManager LvlManager;
        [SerializeField] SOInterger MainMenuStateIndex, GamePlayStateIndex, MinLvlNum;
        [SerializeField] Transform FillImage;

        float _loadingTime = 2;

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
            FillImage.DOScaleX(1, _loadingTime).SetEase(Ease.Linear).OnComplete(()=> {
                if (LvlNum.Value <= MinLvlNum.Value)
                {
                    InitLevelEvent.InvokeSOEvent();
                    ActiveStateEvent.InvokeSOEvent(GamePlayStateIndex.Value);
                }
                else
                {
                    ActiveStateEvent.InvokeSOEvent(MainMenuStateIndex.Value);
                }
                DestroyStateEvent.InvokeSOEvent(0);
                LvlManager.AfterEnable();
            });
        }
    }
}
