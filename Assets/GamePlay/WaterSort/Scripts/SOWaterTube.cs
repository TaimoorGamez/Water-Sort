using UnityEngine;

namespace Core.GamePlay.WaterSort
{
    [CreateAssetMenu(fileName = "WaterTube", menuName = "ScriptableObjects/WaterSort/WaterTube")]
    public class SOWaterTube : ScriptableObject
    {
        public TubeHandler Tube;
    }
}
