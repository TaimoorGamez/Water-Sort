using UnityEngine;
using UnityEngine.UI;


namespace Core.Screen
{
    public class ImageItemView : ItemView
    {
        [SerializeField] Color32  UnselectColor;
        [SerializeField] Image ItemImg;

        Color _selectColor = Color.white;

        public void OnClick()
        {
            ChangeItemStatusEvent.InvokeSOEvent(MyData.ItemId);
        }

        public override void UnSelectItem()
        {
            base.UnSelectItem();
            ItemImg.color = UnselectColor;
        }

        public override void SelectItem()
        {
            base.SelectItem();
            ItemImg.color = _selectColor;
        }

        public override void ActiveSelectItem()
        {
            base.ActiveSelectItem();
            ItemImg.color = _selectColor;
        }
    }
}
