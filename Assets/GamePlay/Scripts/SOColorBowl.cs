using UnityEngine;

namespace Core.GamePlay.Coloring
{
    [CreateAssetMenu(fileName = "ColorBowl", menuName = "ScriptableObjects/Coloring/ColorBowl")]
    public class SOColorBowl : ScriptableObject
    {
        public BowlColorHandler Bowl;
    }
}
