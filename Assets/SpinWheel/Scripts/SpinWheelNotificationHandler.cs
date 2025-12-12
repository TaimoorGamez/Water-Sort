using UnityEngine;
using Core.Events;
using Core.DB.Variables;

namespace Core.Screen
{
    public class SpinWheelNotificationHandler : MonoBehaviour
    {
        [SerializeField] DBInt DailySpin;
        [SerializeField] GameObject NotificationObj;

        private void OnEnable()
        {
            UpdateNotificationStatus();
            SimpleEventsHolder.ResetSpinWheelEvent += ResetSpinWheel;
        }

        private void OnDisable()
        {
            SimpleEventsHolder.ResetSpinWheelEvent -= ResetSpinWheel;
        }

        void ResetSpinWheel()
        {
            DailySpin.Value = 0; // Reset daily spins to 1
            UpdateNotificationStatus();
        }

        void UpdateNotificationStatus()
        {
            NotificationObj.SetActive(DailySpin.Value == 0);
        }
    }
}
