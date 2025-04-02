using UnityEngine;
using Core.Events;
using Core.GamePlay;
using Core.Variables;
using Core.DB.Variables;

namespace Core.Screen
{
    public class SplashScreen : MonoBehaviour
    {
        [SerializeField] ItemData DefaultCap;
        [SerializeField] SOEvents InitLevelEvent;
        [SerializeField] DBInt LvlNum, FFT;
        [SerializeField] SOIntegerEvents ActiveStateEvent, DestroyStateEvent;
        [SerializeField] LevelManager LvlManager;
        [SerializeField] SOInterger MainMenuStateIndex, GamePlayStateIndex, MinLvlNum;

        private void Start()
        {
            if (FFT.Value != 1)
            {
                DefaultCap.IsPurchased = true;
                FFT.Value = 1;
            }

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
        }
    }
}
