using UnityEngine;
using Core.Variables;
using Core.DB.Variables;

namespace Core.GamePlay.WaterSort
{
    public class WaterSortLevelManager : MonoBehaviour
    {
        [SerializeField] WaterSortLevelInit LevelMaker;
        [SerializeField] SOInterger CompletedTubes;
        [SerializeField] DBInt LvlNum;

        int _curentLvl;

        private void Start()
        {
            LevelMaker.InitNewLevel();
        }

        void CheckComplete()
        {
            if (CompletedTubes.Value == _curentLvl)
            {
                LvlNum.Value++;
                CompletedTubes.Value = 0;
            }
        }
    }
}
