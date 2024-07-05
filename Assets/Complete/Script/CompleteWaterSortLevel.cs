using UnityEngine;
namespace Core.States
{
    [CreateAssetMenu(fileName = "WaterSortCompleteLevel", menuName = "ScriptableObjects/States/WaterSortCompleteLevel")]
    public class CompleteWaterSortLevel : GameState
    {
        public override void ActiveCurrentState(Transform parent)
        {
            base.ActiveCurrentState(parent);
        }

        public override void DeActiveCurrentState()
        {

        }

        public override void DestroyCurrentState()
        {
            base.DestroyCurrentState();
        }

        protected override void ShowStateAgain()
        {

        }
    }
}

