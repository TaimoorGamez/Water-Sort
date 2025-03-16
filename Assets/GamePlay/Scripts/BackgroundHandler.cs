using UnityEngine;
using Core.Events;
using Core.Variables;

namespace Core.Screen
{
    public class BackgroundHandler : MonoBehaviour
    {
        [SerializeField] SOInterger MainMenuIndex, GamePlayIndex;
        [SerializeField] SOIntegerEvents BackgroungChangingEvent;
        [SerializeField] GameObject MMbg, GPbg;

        private void OnEnable()
        {
            BackgroungChangingEvent.EventHandler += ChangeBackground;
        }
        private void OnDisable()
        {
            BackgroungChangingEvent.EventHandler -= ChangeBackground;
        }

        void ChangeBackground(int bgNum) 
        {
            if(bgNum == MainMenuIndex.Value)
            {
                MMbg.SetActive(true);
                GPbg.SetActive(false);
            }
            else if (bgNum == GamePlayIndex.Value)
            {
                GPbg.SetActive(true);
                MMbg.SetActive(false);
            }
        }
    }
}
