namespace Core.SpinWheel
{
    public interface ISpinWheel
    {
        void Spin();
    }

    [System.Serializable]
    public class SpinWheelConfige
    {
        public UnityEngine.Gradient SegmentGradient;
        public UnityEngine.Sprite Icon;
        public int Amount;
        public float Weight;
    }
}

