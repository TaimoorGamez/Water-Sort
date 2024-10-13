using UnityEngine;
using Core.Events;
using Core.GamePlay;
using Core.Variables;
using Core.DB.Variables;

namespace Core.Screen
{
    public class SplashScreen : MonoBehaviour
    {
        [SerializeField] DBInt LvlNum;
        [SerializeField] SOIntegerEvents ChangeStateEvent;
        [SerializeField] LevelManager LvlManager;
        [SerializeField] SOInterger MainMenuStateIndex, GamePlayStateIndex;

        int _tutorialLevels = 5;

        private void Start()
        {
            if (LvlNum.Value < _tutorialLevels)
            {
                ChangeStateEvent.InvokeEvent(GamePlayStateIndex.Value);
            }
            else
            {
                ChangeStateEvent.InvokeEvent(MainMenuStateIndex.Value);
            }

            LvlManager.AfterEnable();
        }
    }
}
