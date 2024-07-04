using UnityEngine;
using Core.Events;

namespace Core.Screen
{
    public class WaterSortGameScreen : MonoBehaviour
    {
        [SerializeField] SOEvents InitLevelEvent;

        private void OnEnable()
        {
            InitLevelEvent.InvokeEvent();
        }

        
    }
}
