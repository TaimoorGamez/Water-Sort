using UnityEngine;
using Core.Events;
using Core.DB.Variables;

namespace Core.Screen
{
    public class SplashScreen : MonoBehaviour
    {
        [SerializeField] DBInt LvlNum;
        [SerializeField] SOIntegerEvents ChangeStateEvent;

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
        }
    }
}
