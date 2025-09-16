using UnityEngine;

namespace Core.Store
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
            get => PlayerPrefs.GetInt(ItemName + "Videos" + ItemId, 0);
            set => PlayerPrefs.SetInt(ItemName + "Videos" + ItemId, Mathf.Clamp(value, 0, TotalVideos));
        }

        public int AvailableCards
        {
            get => PlayerPrefs.GetInt(ItemName + "Cards" + ItemId, 0);
            set => PlayerPrefs.SetInt(ItemName + "Cards" + ItemId, Mathf.Clamp(value, 0, TotalCards));
        }
    }
}
