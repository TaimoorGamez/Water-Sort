using Core.Events;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Core.States
{
    public class StateManager : MonoBehaviour
    {
        public int MainMenuStateIndex, GamePlayStateIndex, PauseStateIndex, LevelFailStateIndex, LevelCompleteStateIndex;

        [SerializeField] GameState[] AllStates;

        int _sceneCounter = 0, _maxSceneCount = 5;

        public static StateManager I { get; private set; }

        private void Awake()
        {
            if (I == null)
            {
                I = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }


        private void OnEnable()
        {
            SingleIntegerEventsHolder.ActiveStateEvent += ActiveeState;
            SingleIntegerEventsHolder.DeActiveStateEvent += DeactiveState;
            SingleIntegerEventsHolder.DestroyStatEvent += DestroyState;
        }
        private void OnDisable()
        {
            SingleIntegerEventsHolder.ActiveStateEvent -= ActiveeState;
            SingleIntegerEventsHolder.DeActiveStateEvent -= DeactiveState;
            SingleIntegerEventsHolder.DestroyStatEvent -= DestroyState;
        }

        private void Start()
        {
            AllStates[0].ActiveCurrentState(transform);
        }

        void ActiveeState(int stateIndex)
        {
            AllStates[stateIndex].ActiveCurrentState(transform);
            if (stateIndex == MainMenuStateIndex)
            {
                StartCoroutine(ClearMemoryRoutine());
            }
        }

        void DeactiveState(int stateIndex)
        {
            AllStates[stateIndex].DeactiveState();
        }

        void DestroyState(int stateIndex)
        {
            AllStates[stateIndex].DestroyState();
        }

        IEnumerator ClearMemoryRoutine()
        {
            DG.Tweening.DOTween.KillAll();

            yield return Resources.UnloadUnusedAssets();

            System.GC.Collect();
            yield return null;

            _sceneCounter++;
            if (_sceneCounter > _maxSceneCount)
            {
                SceneManager.LoadScene(0);
                yield break;
            }
        }
    }
}
