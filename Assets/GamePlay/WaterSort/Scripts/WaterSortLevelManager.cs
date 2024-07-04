using UnityEngine;
using Core.Variables;
using Core.DB.Variables;

namespace Core.GamePlay.WaterSort
{
    public class WaterSortLevelManager : MonoBehaviour
    {
        [SerializeField] WaterSortLevelInit LevelMaker;
        [SerializeField] SOInterger CompletedTubes, CurrrentLvl;
        [SerializeField] DBInt LvlNum;

        void CheckComplete()
        {
            if (CompletedTubes.Value == CurrrentLvl.Value)
            {
                LvlNum.Value++;
                CompletedTubes.Value = 0;
            }
        }
    }
}
