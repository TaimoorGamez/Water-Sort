using UnityEngine;

namespace Core.GamePlay
{
    [CreateAssetMenu(fileName = "NewItem", menuName = "ScriptableObjects/Store/ItemData")]
    public class ItemData : ScriptableObject
    {
        public int ItemId, TotalVideos, TotalCards;

        string _itemName = "Cap";

        public bool IsPurchased
        {
            get => PlayerPrefs.GetInt(_itemName + ItemId, 0) == 1;
            set => PlayerPrefs.SetInt(_itemName + ItemId, value ? 1 : 0);
        }

        public int WatchedVideos
        {
            get => PlayerPrefs.GetInt(_itemName + "_Videos_" + ItemId, 0);
            set => PlayerPrefs.SetInt(_itemName + "_Videos_" + ItemId, Mathf.Clamp(value, 0, TotalVideos));
        }

        public int AvailableCards
        {
            get => PlayerPrefs.GetInt(_itemName + "_Cards_" + ItemId, 0);
            set => PlayerPrefs.SetInt(_itemName + "_Cards_" + ItemId, Mathf.Clamp(value, 0, TotalCards));
        }
    }
}
