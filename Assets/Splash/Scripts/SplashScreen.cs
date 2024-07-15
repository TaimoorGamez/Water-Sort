using UnityEngine;
using Core.Events;
using Core.DB.Variables;
using Core.GamePlay.WaterSort;

namespace Core.Screen
{
    public class SplashScreen : MonoBehaviour
    {
        [SerializeField] DBInt LvlNum;
        [SerializeField] SOIntegerEvents ChangeStateEvent;
        [SerializeField] WaterSortLevelManager LvlManager;

        int _tutorialLevels = 5;

        private void Start()
        {
            if (LvlNum.Value < _tutorialLevels)
            {
                ChangeStateEvent.InvokeEvent(2);
            }
            else
            {
                ChangeStateEvent.InvokeEvent(2);
            }

            LvlManager.AfterEnable();
        }
    }
}
