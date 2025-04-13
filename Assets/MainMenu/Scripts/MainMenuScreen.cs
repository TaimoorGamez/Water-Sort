using UnityEngine;
using Core.Events;
using Core.Plugins;
using Core.Variables;

namespace Core.Screen
{
    public class MainMenuScreen : MonoBehaviour
    {
        [SerializeField] Initialization FirebaseInit, AdmobInit;
        [SerializeField] SOEvents InitLevelEvent;
        [SerializeField] SOInterger MainMenuStateIndex, GamePlayStateIndex, SettingStateIndex, TempLvlIndex, IsFirebaseInit;
        [SerializeField] SOIntegerEvents ActiveStateEvent, DestroyStateEvent, ChangeBackgroundEvent;

        private void OnEnable()
        {
            ChangeBackgroundEvent.InvokeSOEvent(MainMenuStateIndex.Value);
        }

        public void OnclickSettingBtn()
        {
            ActiveStateEvent.InvokeSOEvent(SettingStateIndex.Value);
        }

        private void Start()
        {
            TempLvlIndex.Value = -1;
            if(IsFirebaseInit.Value == 1)
            {
                AdmobInit.InitPlugin();
            }
            else
            {
                FirebaseInit.InitPlugin();
            }
        }

        public void OnClickPlayButton()
        {
            InitLevelEvent.InvokeSOEvent();
            ActiveStateEvent.InvokeSOEvent(GamePlayStateIndex.Value);
            DestroyStateEvent.InvokeSOEvent(MainMenuStateIndex.Value);
        }
    }
}
