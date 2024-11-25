using UnityEngine;
using Core.Events;
using Core.Variables;

namespace Core.GamePlay.Coloring
{
    public class BowlColorHandler : MonoBehaviour
    {
        [SerializeField] SOEvents ColorSelectionEvent; 
        [SerializeField] SOColor CurrentColor;
        [SerializeField] SOColorBowl CurrentBowl;
        [SerializeField] Renderer MySkin;

        MaterialPropertyBlock _propBlock;
        Vector3 _orignalPos;
        Color _bowlColor;

        public void SetColor(Color currentColor)
        {
            _propBlock = new MaterialPropertyBlock();
            _bowlColor = currentColor;
            _orignalPos = transform.position;
            _propBlock.SetColor("_BaseColor", currentColor);
            _propBlock.SetFloat("_TransparencyRange", 1);
            MySkin.SetPropertyBlock(_propBlock);
        }
        private void OnMouseDown()
        {
            if (CurrentBowl.Bowl == null)
            {
                CurrentBowl.Bowl = this;
                BowlState(true);
            }
            else
            {
                CurrentBowl.Bowl.BowlState(false);
                CurrentBowl.Bowl = this;
                BowlState(true);
            }
            CurrentColor.Value = _bowlColor;
            ColorSelectionEvent.InvokeSOEvent();
        }

        public void BowlState(bool state)
        {
            if (state)
            {
                _propBlock.SetInteger("_Glow", 1);
                LeanTween.moveLocalY(gameObject, _orignalPos.y + 0.5f, 0.1f);
            }
            else
            {
                _propBlock.SetInteger("_Glow", 0);
                LeanTween.moveLocal(gameObject, _orignalPos, 0.05f);
            }
            MySkin.SetPropertyBlock(_propBlock);
        }
    }
}
