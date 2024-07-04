using UnityEngine;

namespace Core.States
{
    public class GameState : ScriptableObject
    {
        [SerializeField] protected GameObject StateViewPrefab;

        protected GameObject stateView;

        public virtual void ActiveCurrentState(Transform parent)
        {
            if (stateView == null)
            {
                stateView = Instantiate(StateViewPrefab, parent);
            }
            else
            {
                ShowStateAgain();
            }
        }

        public virtual void DeActiveCurrentState()
        {

        }

        public virtual void DestroyCurrentState()
        {
            if (stateView != null)
                Destroy(stateView);
        }

        protected virtual void ShowStateAgain()
        {

        }
    }
}
