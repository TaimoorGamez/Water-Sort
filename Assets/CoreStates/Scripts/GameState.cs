using UnityEngine;

namespace Core.States
{
    [CreateAssetMenu(fileName = "ScreenState", menuName = "ScriptableObjects/State")]
    public class GameState : ScriptableObject
    {
        [SerializeField] GameObject StateViewPrefab;

        GameObject _stateScreen;

        public void ActiveCurrentState(Transform parent)
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

        public void DeActiveCurrentState()
        {
            _stateScreen.SetActive(false);
        }

        public void DestroyCurrentState()
        {
            if (_stateScreen != null)
                Destroy(_stateScreen);
        }

        void ShowStateAgain()
        {
            _stateScreen.SetActive(true);
        }
    }
}
