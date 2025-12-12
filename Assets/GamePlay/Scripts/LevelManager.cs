using UnityEngine;
using Core.Events;
using Core.Variables;
using Core.DB.Variables;

namespace Core.GamePlay
{
    [CreateAssetMenu(fileName = "LevelManager", menuName = "ScriptableObjects/WaterSort/LevelManager")]
    public class LevelManager : ScriptableObject
    {
        [SerializeField] SOInterger CompletedTubes, CurrrentLvl, CanPlay, LevelCompleteStateIndex, TotalMoves, SortingCompleted;
        [SerializeField] DBInt LvlNum;

        public void AfterEnable()
        {
            SimpleEventsHolder.CheckCompleteEvent = CheckComplete;
            SimpleEventsHolder.RegisterMoveEvent = CheckMoves;
        }

        void CheckComplete()
        {
            if (CompletedTubes.Value == CurrrentLvl.Value)
            {
                CompletedTubes.Value = 0;
                SortingCompleted.Value = 1;
                SimpleEventsHolder.StartColoringEvent?.Invoke();
            }
        }

        void CheckMoves()
        {
            TotalMoves.Value--;
            SimpleEventsHolder.UpdateMovesEvent?.Invoke();
        }
    }
}
