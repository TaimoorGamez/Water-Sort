using Core.DB.Variables;

namespace Core.SpinWheel
{
    public interface ISpinWheel
    {
        void Spin();
    }

    [System.Serializable]
    public class SpinWheelConfige
    {
        public DBInteger RewardDbs;
        public UnityEngine.Color SegmentColor;
        public UnityEngine.Sprite Icon;
        public int Amount;
        public float Weight;

        public void ClaimReward()
        {
            RewardDbs.Value += Amount;
        }
    }
}

