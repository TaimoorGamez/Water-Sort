using Core.Events;
using UnityEngine;
using UnityEngine.UI;


namespace Core.Store
{ public class ImageItemView : ItemView
    {
        [SerializeField] Color32 SelectColor, UnselectColor;
        [SerializeField] Image ItemImg;

        public void OnClick()
        {
            SingleIntegerEventsHolder.UpdateItemStatusEvent?.Invoke(MyData.ItemId);
        }

        public override void UnSelectItem()
        {
            base.UnSelectItem();
            ItemImg.color = UnselectColor;
        }

        public override void SelectItem()
        {
            base.SelectItem();
            ItemImg.color = SelectColor;
        }

        public override void ActiveSelectItem()
        {
            base.ActiveSelectItem();
            ItemImg.color = SelectColor;
        }
    }
}
