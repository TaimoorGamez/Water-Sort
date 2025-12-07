using UnityEngine;

namespace Core.SpinWheel
{
    [CreateAssetMenu(fileName = "SpinWheelData", menuName = "ScriptableObjects/SpinWheel/Data")]
    public class SpinWheelData : ScriptableObject
    {
        public SpinWheelConfige[] SpinWheelRewards;
    }
}
