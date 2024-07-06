using UnityEngine;
using Core.Events;

namespace Core.Screen
{
    public class WaterSortGameScreen : MonoBehaviour
    {
        [SerializeField] GameObject SwipeColorsModePanel;
        [SerializeField] SOEvents InitLevelEvent, SwipeColorsModeEvent;

        private void OnEnable()
        {
            InitLevelEvent.InvokeEvent();
            SwipeColorsModeEvent.EventHandler += SwitchSwipeColorsMode;
            SwipeColorsModePanel.SetActive(true);
        }

        private void OnDisable()
        {
            SwipeColorsModeEvent.EventHandler -= SwitchSwipeColorsMode;
        }

        void SwitchSwipeColorsMode()
        {
            SwipeColorsModePanel.SetActive(!SwipeColorsModePanel.activeInHierarchy);
        }
        
    }
}
