using UnityEngine;
using Core.Events;
using Core.Variables;
using System.Collections;
using UnityEngine.SceneManagement;

namespace Core.States
{
    public class StateManager : MonoBehaviour
    {
        [SerializeField] SOIntegerEvents ActiveStateEvent, DeactiveStateEvent, DestroyStateEvent;
        [SerializeField] GameState[] AllStates;
        [SerializeField] SOInterger MainMenuStateIndex, GamePlayStateIndex, LeveCompleteStateIndex;

        int _sceneCounter = 0, _maxSceneCount = 5;

        private void OnEnable()
        {
            ActiveStateEvent.EventHandler += ActiveeState;
            DeactiveStateEvent.EventHandler += DeactiveState;
            DestroyStateEvent.EventHandler += DestroyState;
        }
        private void OnDisable()
        {
            ActiveStateEvent.EventHandler -= ActiveeState;
            DeactiveStateEvent.EventHandler -= DeactiveState;
            DestroyStateEvent.EventHandler -= DestroyState;
        }

        private void Start()
        {
            AllStates[0].ActiveCurrentState(transform);
        }

        void ActiveeState(int stateIndex)
        {
            AllStates[stateIndex].ActiveCurrentState(transform);
            if (stateIndex == MainMenuStateIndex.Value)
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
