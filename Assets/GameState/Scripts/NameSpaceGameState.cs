using UnityEngine;

namespace Core.States
{
    public class GameState : ScriptableObject
    {
        [SerializeField] protected GameObject StateViewPrefab;

        protected GameObject _stateScreen;

        public virtual void ActiveCurrentState(Transform parent)
        {
            if (_stateScreen == null)
            {
                _stateScreen = Instantiate(StateViewPrefab, parent);
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
            if (_stateScreen != null)
                Destroy(_stateScreen);
        }

        protected virtual void ShowStateAgain()
        {

        }
    }
}
