using UnityEngine;
using Core.Events;
using Core.Variables;
using Core.DB.Variables;

namespace Core.GamePlay.WaterSort
{
    [CreateAssetMenu(fileName = "LevelManager", menuName = "ScriptableObjects/WaterSort/LevelManager")]
    public class WaterSortLevelManager : ScriptableObject
    {
        [SerializeField] SOInterger CompletedTubes, CurrrentLvl;
        [SerializeField] DBInt LvlNum;
        [SerializeField] SOEvents CheckCompleteEvent;
        [SerializeField] SOIntegerEvents ChangeStateEvent;

        private void OnEnable()
        {
            CheckCompleteEvent.EventHandler += CheckComplete;
        }

        private void OnDisable()
        {
            CheckCompleteEvent.EventHandler -= CheckComplete;
        }

        void CheckComplete()
        {
            if (CompletedTubes.Value == CurrrentLvl.Value)
            {
                ChangeStateEvent.InvokeEvent(3);
                LvlNum.Value++;
                CompletedTubes.Value = 0;
            }
        }
    }
}
