using UnityEngine;
using Core.Events;
using Core.Variables;

namespace Core.States
{
    public class StateManager : MonoBehaviour
    {
        [SerializeField] SOIntegerEvents ActiveStateEvent, DeactiveStateEvent, DestroyStateEvent;
        [SerializeField] GameState[] AllStates;
        [SerializeField] SOInterger MainMenuStateIndex, GamePlayStateIndex, LeveCompleteStateIndex;

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
        }

        void DeactiveState(int stateIndex)
        {
            AllStates[stateIndex].DeactiveState();
        }

        void DestroyState(int stateIndex)
        {
            AllStates[stateIndex].DestroyState();
        }
    }
}
