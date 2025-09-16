using UnityEngine;
using Core.Events;
using Core.DB.Variables;

namespace Core.Screen
{
    public class SpinWheelNotificationHandler : MonoBehaviour
    {
        [SerializeField] SOEvents ResetSpinWheelEvent;
        [SerializeField] DBInt DailySpin;
        [SerializeField] GameObject NotificationObj;

        private void OnEnable()
        {
            UpdateNotificationStatus();
            ResetSpinWheelEvent.EventHandler += ResetSpinWheel;
        }

        private void OnDisable()
        {
            ResetSpinWheelEvent.EventHandler -= ResetSpinWheel;
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
