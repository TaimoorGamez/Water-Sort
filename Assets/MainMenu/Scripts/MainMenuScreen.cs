using UnityEngine;
using Core.Events;
using Core.Variables;

namespace Core.Screen
{
    public class MainMenuScreen : MonoBehaviour
    {
        [SerializeField] SOInterger MainMenuStateIndex, GamePlayStateIndex;
        [SerializeField] SOIntegerEvents ActiveStateEvent, DestroyStateEvent;

        public void OnClickPlayButton()
        {
            ActiveStateEvent.InvokeSOEvent(GamePlayStateIndex.Value);
            DestroyStateEvent.InvokeSOEvent(MainMenuStateIndex.Value);
        }
    }
}
