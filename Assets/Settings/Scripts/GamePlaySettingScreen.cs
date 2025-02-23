using UnityEngine;
using Core.Events;
using Core.Variables;
using Core.Animations.DT;

namespace Core.Screen
{
    public class GamePlaySettingScreen : MonoBehaviour
    {
        [SerializeField] SODOTween PopScaleUp, PopScaleDown;
        [SerializeField] GameObject PopPanel;
        [SerializeField] SOIntegerEvents ActiveStateEvent;
        [SerializeField] SOInterger MainMenuStateIndex, GamePlayStateIndex;

        private void OnEnable()
        {
            PopScaleUp.TargetObj = PopPanel;
            PopScaleUp.PlayAnimation();
        }

        public void ClosePopPanel()
        {
            PopScaleDown.TargetObj = PopPanel;
            PopScaleDown.PlayAnimation();
            Invoke(nameof(DisableObject),0.45f);
        }

        void DisableObject()
        {
            ActiveStateEvent.InvokeSOEvent(GamePlayStateIndex.Value);
        }

        public void BackToHome()
        {
            PopScaleDown.TargetObj = PopPanel;
            PopScaleDown.PlayAnimation();
            Invoke(nameof(DestroyState), 0.45f);
        }

        void DestroyState()
        {
            ActiveStateEvent.InvokeSOEvent(MainMenuStateIndex.Value);
        }
    }
}
