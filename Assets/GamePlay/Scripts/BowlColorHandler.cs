using UnityEngine;
using DG.Tweening;
using Core.Events;
using Core.Variables;

namespace Core.GamePlay.Coloring
{
    public class BowlColorHandler : MonoBehaviour
    {
        [SerializeField] SOIntegerEvents SoundEffectEvent;
        [SerializeField] SOEvents ColorSelectionEvent, HideColorBowlEvent; 
        [SerializeField] SOColor CurrentColor;
        [SerializeField] SOColorBowl CurrentBowl;
        [SerializeField] Renderer MySkin;
        [SerializeField] ParticleSystem WaveParticle;

        MaterialPropertyBlock _propBlock;
        Vector3 _orignalPos;
        Color _bowlColor;

        private void OnEnable()
        {
            HideColorBowlEvent.EventHandler += HideNow;
        }

        private void OnDisable()
        {
            HideColorBowlEvent.EventHandler -= HideNow;
        }

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
            Debug.Log("OnMouseDown triggered on bowl: ");
            if (CurrentBowl.Bowl == null)
            {
                Debug.Log("CurrentBowl reference is null!");
                CurrentBowl.Bowl = this;
                BowlState(true);
            }
            else
            {
                Debug.Log("Changing selection from ");
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
                SoundEffectEvent.InvokeSOEvent(0);
                _propBlock.SetInteger("_Glow", 1);
                transform.DOLocalMoveY(_orignalPos.y + 0.2f, 0.1f);
                WaveParticle.Play();
            }
            else
            {
                _propBlock.SetInteger("_Glow", 0);
                transform.DOLocalMove(_orignalPos, 0.05f);
                WaveParticle.Stop();
            }
            MySkin.SetPropertyBlock(_propBlock);
        }

        void HideNow()
        {
            gameObject.SetActive(false);
        }
    }
}
