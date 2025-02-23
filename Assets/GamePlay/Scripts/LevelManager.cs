using UnityEngine;
using Core.Events;
using Core.Variables;
using Core.DB.Variables;

namespace Core.GamePlay
{
    [CreateAssetMenu(fileName = "LevelManager", menuName = "ScriptableObjects/WaterSort/LevelManager")]
    public class LevelManager : ScriptableObject
    {
        [SerializeField] SOInterger CompletedTubes, CurrrentLvl, CanPlay, LevelCompleteStateIndex;
        [SerializeField] DBInt LvlNum;
        [SerializeField] SOEvents CheckCompleteEvent, StartColoringEvent;

        public void AfterEnable()
        {
            CheckCompleteEvent.EventHandler = CheckComplete;
        }

        void CheckComplete()
        {
            if (CompletedTubes.Value == CurrrentLvl.Value)
            {
                CompletedTubes.Value = 0;
                StartColoringEvent.InvokeSOEvent();
            }
        }
    }
}
