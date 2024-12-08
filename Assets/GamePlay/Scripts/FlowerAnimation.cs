using UnityEngine;
using DG.Tweening;
using System.Collections;


namespace Core.GamePlay.WaterSort
{
    public class FlowerAnimation : CapAnimation
    {
        [SerializeField] Renderer MySkin;
        [SerializeField] Texture MyTexture;

        float _transparencyChangeDuration = 0.5f, _targetTransparency = 1;
        MaterialPropertyBlock _propBlock;
        Coroutine _animationRotine;


        public override void PlayCapAnimation(Color currentColor)
        {
            _propBlock = new MaterialPropertyBlock();
            _propBlock.SetColor("_BaseColor", currentColor);
            _propBlock.SetTexture("_MainTex", MyTexture);
            _propBlock.SetFloat("_ColorRange", 0);
            MySkin.SetPropertyBlock(_propBlock);
            _animationRotine = StartCoroutine(SmoothlyChangeColor());
            transform.DOLocalMoveZ(0, _transparencyChangeDuration);
        }

        IEnumerator SmoothlyChangeColor()
        {
            float elapsedTime = 0f, startTransparency = 0, smoothTimer = 0.01f;
            while (elapsedTime < _transparencyChangeDuration)
            {
                elapsedTime += smoothTimer;
                float t = Mathf.Clamp01(elapsedTime / _transparencyChangeDuration);
                float currentTransparency = Mathf.Lerp(startTransparency, _targetTransparency, t);
                _propBlock.SetFloat("_ColorRange", currentTransparency);
                MySkin.SetPropertyBlock(_propBlock);
                yield return new WaitForSeconds(smoothTimer);
            }

            if (_animationRotine != null)
            {
                StopCoroutine(_animationRotine);
            }
        }

        private void OnDisable()
        {
            //LeanTween.kill
            if (_animationRotine != null)
            {
                StopCoroutine(_animationRotine);
            }
        }
    }
}
