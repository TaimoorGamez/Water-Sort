using UnityEngine;

namespace Core.Plugins.Ads
{
    [CreateAssetMenu(fileName = "AdData", menuName = "ScriptableObjects/Plugin/Admob/AdData")]
    public class AdDataHandler : ScriptableObject
    {
        public AdConfig AdData;
    }
}
