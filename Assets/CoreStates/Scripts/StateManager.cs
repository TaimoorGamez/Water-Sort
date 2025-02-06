using UnityEngine;
using Core.Events;
using Core.Variables;

namespace Core.States
{
    public class StateManager : MonoBehaviour
    {
        [SerializeField] SOEvents ChangeBackgroundEvent;
        [SerializeField] SOIntegerEvents ChangeStateEvent, DeactiveStateEvent, DestroyStateEvent;
        [SerializeField] GameState[] AllStates;
        [SerializeField] SOInterger MainMenuStateIndex, GamePlayStateIndex, LeveCompleteStateIndex;

        private void OnEnable()
        {
            ChangeStateEvent.EventHandler += ChangeState;
            DeactiveStateEvent.EventHandler += DeactiveState;
            DestroyStateEvent.EventHandler += DestroyState;
        }
        private void OnDisable()
        {
            ChangeStateEvent.EventHandler -= ChangeState;
            DeactiveStateEvent.EventHandler -= DeactiveState;
            DestroyStateEvent.EventHandler -= DestroyState;
        }

        private void Start()
        {
            AllStates[0].ActiveCurrentState(transform);
        }

        void ChangeState(int stateIndex)
        {
            if (stateIndex == MainMenuStateIndex.Value)
            {
                ChangeBackgroundEvent.InvokeSOEvent();
                for (int s = 0; s > AllStates.Length; s++)
                {
                    AllStates[s].DestroyState();
                }
            }
            else if (stateIndex == GamePlayStateIndex.Value)
            {
                for (int s = 0; s < AllStates.Length; s++)
                {
                    AllStates[s].DestroyState();
                }
            }
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
