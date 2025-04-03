using UnityEngine;

namespace Core.GamePlay
{
    [CreateAssetMenu(fileName = "NewItem", menuName = "ScriptableObjects/Store/ItemData")]
    public class ItemData : ScriptableObject
    {
        public int ItemId, TotalVideos, TotalCards;

        [SerializeField] string ItemName;

        public bool IsPurchased
        {
            get => PlayerPrefs.GetInt(ItemName + ItemId, 0) == 1;
            set => PlayerPrefs.SetInt(ItemName + ItemId, value ? 1 : 0);
        }

        public int WatchedVideos
        {
            get => PlayerPrefs.GetInt(ItemName + "_Videos_" + ItemId, 0);
            set => PlayerPrefs.SetInt(ItemName + "_Videos_" + ItemId, Mathf.Clamp(value, 0, TotalVideos));
        }

        public int AvailableCards
        {
            get => PlayerPrefs.GetInt(ItemName + "_Cards_" + ItemId, 0);
            set => PlayerPrefs.SetInt(ItemName + "_Cards_" + ItemId, Mathf.Clamp(value, 0, TotalCards));
        }
    }
}
