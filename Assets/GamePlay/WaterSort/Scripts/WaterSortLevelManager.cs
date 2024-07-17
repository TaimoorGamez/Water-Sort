using UnityEngine;
using Core.Events;
using Core.Variables;
using Core.DB.Variables;

namespace Core.GamePlay.WaterSort
{
    [CreateAssetMenu(fileName = "LevelManager", menuName = "ScriptableObjects/WaterSort/LevelManager")]
    public class WaterSortLevelManager : ScriptableObject
    {
        [SerializeField] SOInterger CompletedTubes, CurrrentLvl, CanPlay, LevelCompleteStateIndex;
        [SerializeField] DBInt LvlNum;
        [SerializeField] SOEvents CheckCompleteEvent;
        [SerializeField] SOIntegerEvents ChangeStateEvent;

        public void AfterEnable()
        {
            CheckCompleteEvent.EventHandler = CheckComplete;
            //Debug.Log("Here23");
        }

        void CheckComplete()
        {
            //Debug.Log("Here27");
            if (CompletedTubes.Value == CurrrentLvl.Value)
            {
                //Debug.Log("Here30");
                CanPlay.Value = 0;
                ChangeStateEvent.InvokeEvent(LevelCompleteStateIndex.Value);
                LvlNum.Value++;
                CompletedTubes.Value = 0;
            }
        }
    }
}
