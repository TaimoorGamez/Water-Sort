using UnityEngine;

namespace Core.States
{
    [CreateAssetMenu(fileName = "SplashState", menuName = "ScriptableObjects/States/Splash")]
    public class SplashState : GameState
    {
        public override void ActiveCurrentState(Transform parent)
        {
            base.ActiveCurrentState(parent);
        }

        public override void DeActiveCurrentState()
        {

        }

        public override void DestroyCurrentState()
        {
            base.DestroyCurrentState();
        }

        protected override void ShowStateAgain()
        {

        }
    }
}
