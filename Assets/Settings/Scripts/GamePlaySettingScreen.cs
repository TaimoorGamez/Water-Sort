using UnityEngine;
using Core.Events;
using Core.Variables;
using Core.Animations.DT;

namespace Core.Screen
{
    public class GamePlaySettingScreen : MonoBehaviour
    {
        [SerializeField] SOLeanTween PopScaleUp, PopScaleDown;
        [SerializeField] GameObject PopPanel;
        [SerializeField] SOIntegerEvents ChangeStateEvent;
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
            ChangeStateEvent.InvokeEvent(GamePlayStateIndex.Value);
        }

        public void BackToHome()
        {
            PopScaleDown.TargetObj = PopPanel;
            PopScaleDown.PlayAnimation();
            Invoke(nameof(DestroyState), 0.45f);
        }

        void DestroyState()
        {
            ChangeStateEvent.InvokeEvent(MainMenuStateIndex.Value);
        }
    }
}
