using System.Collections;
using UnityEngine;

namespace Core.GamePlay.WaterSort
{
    public class CapAnimation : MonoBehaviour
    {
        [SerializeField] Renderer MySkin;
        [SerializeField] Texture MyTexture;

        float _transparencyChangeDuration = 0.5f, _targetTransparency = 1;
        MaterialPropertyBlock _propBlock;
        Coroutine _animationRotine;

        private void Awake()
        {
            _propBlock = new MaterialPropertyBlock();
        }

        public void SetColor(Color currentColor)
        {
            Color initialColor = new Color(1, 1, 1, 0);
            _propBlock.SetColor("_Color", initialColor);
            _propBlock.SetTexture("_MainTex", MyTexture);
            MySkin.SetPropertyBlock(_propBlock);
            _animationRotine = StartCoroutine(SmoothlyChangeColor(currentColor));
            LeanTween.moveLocalZ(gameObject, 0, _transparencyChangeDuration);
        }

        IEnumerator SmoothlyChangeColor(Color currentColor)
        {
            float elapsedTime = 0f;

            while (elapsedTime < _transparencyChangeDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / _transparencyChangeDuration);

                // Interpolate alpha value
                float currentAlpha = Mathf.Lerp(0, 1, t);
                Color initialColor = new Color(1, 1, 1, currentAlpha);

                _propBlock.SetColor("_Color", initialColor);
                MySkin.SetPropertyBlock(_propBlock);
                yield return null;
            }
            yield return new WaitForSeconds(0.1f);
            _propBlock.SetColor("_Color", currentColor);
            _propBlock.SetFloat("_FillAmount", 0);
            float startTransparency = 0;
            elapsedTime = 0f;
            yield return new WaitForSeconds(0.1f);
            while (elapsedTime < _transparencyChangeDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / _transparencyChangeDuration);
                float currentTransparency = Mathf.Lerp(startTransparency, _targetTransparency, t);
                _propBlock.SetFloat("_FillAmount", currentTransparency);
                MySkin.SetPropertyBlock(_propBlock);
                yield return null;
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
