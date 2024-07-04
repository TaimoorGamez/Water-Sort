using UnityEngine;
namespace Core.States
{
    [CreateAssetMenu(fileName = "WaterSortGameState", menuName = "ScriptableObjects/States/WaterSortGame")]
    public class WaterSortGameState : GameState
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

