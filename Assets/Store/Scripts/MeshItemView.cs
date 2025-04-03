using UnityEngine;

namespace Core.Screen
{
    public class MeshItemView : ItemView
    {
        [SerializeField] Color32 SelecteColor, UnselectColor;
        [SerializeField] Renderer MySkin;
        [SerializeField] Texture MyTexture;

        MaterialPropertyBlock _propBlock;

        private void Start()
        {
            _propBlock = new MaterialPropertyBlock();
            _propBlock.SetTexture("_MainTex", MyTexture);
            _propBlock.SetFloat("_ColorRange", 1);
            MySkin.SetPropertyBlock(_propBlock);
        }

        public void OnClick()
        {
            ChangeItemStatusEvent.InvokeSOEvent(MyData.ItemId);
        }

        public override void UnSelectItem()
        {
            _propBlock = new MaterialPropertyBlock();
            base.UnSelectItem();
            _propBlock.SetColor("_BaseColor", UnselectColor);
            MySkin.SetPropertyBlock(_propBlock);
        }

        public override void SelectItem()
        {
            _propBlock = new MaterialPropertyBlock();
            base.SelectItem();
            _propBlock.SetColor("_BaseColor", SelecteColor);
            MySkin.SetPropertyBlock(_propBlock);
        }

        public override void ActiveSelectItem()
        {
            _propBlock = new MaterialPropertyBlock();
            base.ActiveSelectItem();
            _propBlock.SetColor("_BaseColor", SelecteColor);
            MySkin.SetPropertyBlock(_propBlock);
        }
    }
}
