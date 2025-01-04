using Core.Events;
using UnityEngine;

namespace Core.Screen
{
    public class BackgroundHandler : MonoBehaviour
    {
        [SerializeField] SOEvents BackgroungChangingEvent;
        [SerializeField] GameObject[] Backgrounds;

        private void OnEnable()
        {
            BackgroungChangingEvent.EventHandler += ChangeBackground;
        }
        private void OnDisable()
        {
            BackgroungChangingEvent.EventHandler -= ChangeBackground;
        }

        void ChangeBackground() 
        {
            for (int i = 0; i < Backgrounds.Length; i++) { Backgrounds[i].SetActive(false); }
            Backgrounds[Random.Range(0,Backgrounds.Length)].SetActive(true);
        }
    }
}
