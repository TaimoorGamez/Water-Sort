using UnityEngine;
using Core.Events;
using Core.Variables;
using Core.DB.Variables;

namespace Core.GamePlay.WaterSort
{
    public class ColoringLevelInit : MonoBehaviour
    {
        [SerializeField] SOEvents InitLevelEvent, RestartLevelEvent, DestroyLevelEvent;
        [SerializeField] DBInt LvlIndex;
        [SerializeField] SOInterger TempLevelIndex;
        [SerializeField] Transform ColoringHolder;
         
        string _coloringPath = "ColoringPart/lvl ";

        
        private void OnEnable()
        {
            InitLevelEvent.EventHandler += InitColoring;
            RestartLevelEvent.EventHandler += RegenrateColoring;
            DestroyLevelEvent.EventHandler += DestroyColoring;
        }

        private void OnDisable()
        {
            InitLevelEvent.EventHandler -= InitColoring;
            RestartLevelEvent.EventHandler -= RegenrateColoring;
            DestroyLevelEvent.EventHandler -= DestroyColoring;
        }

        void InitColoring()
        {
            Instantiate(Resources.Load(_coloringPath + (TempLevelIndex.Value == -1 ? LvlIndex.Value : TempLevelIndex.Value)), ColoringHolder);
        }
        void RegenrateColoring()
        {
            DestroyColoring();
            InitColoring();
        }
        void DestroyColoring()
        {
            Destroy(ColoringHolder.GetChild(0).gameObject);
        }
    }
}
