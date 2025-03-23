using UnityEngine;
using Core.Events;
using Core.Variables;

namespace Core.Screen
{
    public class MainMenuScreen : MonoBehaviour
    {
        [SerializeField] SOEvents InitLevelEvent;
        [SerializeField] SOInterger MainMenuStateIndex, GamePlayStateIndex, SettingStateIndex, TempLvlIndex;
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
        }

        public void OnClickPlayButton()
        {
            InitLevelEvent.InvokeSOEvent();
            ActiveStateEvent.InvokeSOEvent(GamePlayStateIndex.Value);
            DestroyStateEvent.InvokeSOEvent(MainMenuStateIndex.Value);
        }
    }
}
