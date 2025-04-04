using UnityEngine;
using DG.Tweening;
using Core.Events;

namespace Core.GamePlay.WaterSort
{
    public class FlowerAnimation : CapAnimation
    {
        [SerializeField] SOEvents StopLoopEffect, StartColoringEvent;
        [SerializeField] SOIntegerEvents LoopEffectEvent;
        [SerializeField] Renderer MySkin;
        [SerializeField] Texture MyTexture;

        float _transparencyChangeDuration = 2.5f, _targetTransparency = 1;
        MaterialPropertyBlock _propBlock;

        private void OnEnable()
        {
            StartColoringEvent.EventHandler += HideNow;
        }

        private void OnDisable()
        {
            StartColoringEvent.EventHandler -= HideNow;
        }

        public override void PlayCapAnimation(Color currentColor)
        {
            _propBlock = new MaterialPropertyBlock();
            _propBlock.SetColor("_BaseColor", currentColor);
            _propBlock.SetTexture("_MainTex", MyTexture);
            _propBlock.SetFloat("_ColorRange", 0);
            MySkin.SetPropertyBlock(_propBlock); 
            transform.DOLocalMoveY(0,0.2f).OnComplete(() =>
            {
                LoopEffectEvent.InvokeSOEvent(1);
                DOTween.To(() => 0f, value =>
                {
                    _propBlock.SetFloat("_ColorRange", value);
                    MySkin.SetPropertyBlock(_propBlock);
                }, _targetTransparency, _transparencyChangeDuration).OnComplete(()=> StopLoopEffect.InvokeSOEvent());
            });
        }

        void HideNow()
        {
            gameObject.SetActive(false);
        }

    }
}
