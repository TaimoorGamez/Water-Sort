using UnityEngine;
using Core.Events;

namespace Core.States
{
    public class StateManager : MonoBehaviour
    {
        [SerializeField] SOIntegerEvents ChangeStateEvent;
        [SerializeField] GameState[] AllStates;

        int _waterGameStateId = 2, _mainMenuId = 1, _lastStateId = 0;

        private void OnEnable()
        {
            ChangeStateEvent.EventHandler += ChangeState;
        }
        private void OnDisable()
        {
            ChangeStateEvent.EventHandler -= ChangeState;
        }

        private void Start()
        {
            AllStates[0].ActiveCurrentState(transform);
            _lastStateId = 0;
        }

        void ChangeState(int stateNum)
        {
            AllStates[_lastStateId].DeActiveCurrentState();
            AllStates[stateNum].ActiveCurrentState(transform);
            if (stateNum == _waterGameStateId)
            {
                for (int s = 0; s < _waterGameStateId; s++)
                {
                    AllStates[s].DestroyCurrentState();
                }
            }
            else if (stateNum == _mainMenuId)
            {
                for (int s = AllStates.Length - 1; s > _mainMenuId; s--)
                {
                    AllStates[s].DestroyCurrentState();
                }
            }
            _lastStateId = stateNum;
        }
    }
}
