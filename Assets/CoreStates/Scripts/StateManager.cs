using UnityEngine;
using Core.Events;
using Core.Variables;

namespace Core.States
{
    public class StateManager : MonoBehaviour
    {
        [SerializeField] SOEvents ChangeBackgroundEvent;
        [SerializeField] SOIntegerEvents ChangeStateEvent, DeActiveStateEvent;
        [SerializeField] GameState[] AllStates;
        [SerializeField] SOInterger MainMenuStateIndex, GamePlayStateIndex, LeveCompleteStateIndex;

        private void OnEnable()
        {
            ChangeStateEvent.EventHandler += ChangeState;
            DeActiveStateEvent.EventHandler += DeActiveState;
        }
        private void OnDisable()
        {
            ChangeStateEvent.EventHandler -= ChangeState;
            DeActiveStateEvent.EventHandler -= DeActiveState;
        }

        private void Start()
        {
            AllStates[0].ActiveCurrentState(transform);
        }

        void ChangeState(int stateIndex)
        {
            AllStates[stateIndex].ActiveCurrentState(transform);
            if (stateIndex == MainMenuStateIndex.Value)
            {
                ChangeBackgroundEvent.InvokeSOEvent();
                for (int s = AllStates.Length - 1; s > MainMenuStateIndex.Value; s--)
                {
                    AllStates[s].DestroyCurrentState();
                }
            }
            else if (stateIndex == GamePlayStateIndex.Value)
            {
                for (int s = 0; s < GamePlayStateIndex.Value; s++)
                {
                    AllStates[s].DestroyCurrentState();
                }
            }
        }

        void DeActiveState(int stateIndex)
        {
            AllStates[stateIndex].DeActiveCurrentState();
        }
    }
}
