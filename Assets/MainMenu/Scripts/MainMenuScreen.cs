using UnityEngine;
using Core.Events;
using Core.Variables;

namespace Core.Screen
{
    public class MainMenuScreen : MonoBehaviour
    {
        [SerializeField] SOInterger MainMenuStateIndex, GamePlayStateIndex, SettingStateIndex;
        [SerializeField] SOIntegerEvents ActiveStateEvent, DestroyStateEvent, ChangeBackgroundEvent;

        private void OnEnable()
        {
            ChangeBackgroundEvent.InvokeSOEvent(MainMenuStateIndex.Value);
        }

        public void OnclickSettingBtn()
        {
            ActiveStateEvent.InvokeSOEvent(SettingStateIndex.Value);
        }

        public void OnClickPlayButton()
        {
            ActiveStateEvent.InvokeSOEvent(GamePlayStateIndex.Value);
            DestroyStateEvent.InvokeSOEvent(MainMenuStateIndex.Value);
        }
    }
}
