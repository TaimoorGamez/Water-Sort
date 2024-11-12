using UnityEngine;
using System.Collections;

namespace Core.GamePlay.WaterSort
{
    public class Liquid : MonoBehaviour
    {
        [SerializeField] Renderer MySkin;

        MaterialPropertyBlock _propBlock;
        float _transparencyChangeDuration = 1;

        private void Start()
        {
            _propBlock = new MaterialPropertyBlock();
        }

        public void SetHidenColour()
        {
            _propBlock.SetFloat("_HidenRange", 1);
            MySkin.SetPropertyBlock(_propBlock);
        }

        public void SetColor(Color currentColor)
        {
            _propBlock.SetColor("_BaseColor", currentColor);
            _propBlock.SetFloat("_TransparencyRange", 1);
            MySkin.SetPropertyBlock(_propBlock);
        }

        public void SetGlow(bool State)
        {
            if (State)
            {
                _propBlock.SetInteger("_Glow", 1);
            }
            else
            {
                _propBlock.SetInteger("_Glow", 0);
            }
            MySkin.SetPropertyBlock(_propBlock);
        }

        public void HideColor()
        {
            _propBlock.SetFloat("_TransparencyRange", 0);
            MySkin.SetPropertyBlock(_propBlock);
        }

        public IEnumerator SmoothlyAddColor(Color currentColor, float duration)
        {
            _propBlock.SetColor("_BaseColor", currentColor);
            _propBlock.SetFloat("_TransparencyRange", 0);
            MySkin.SetPropertyBlock(_propBlock);
            float elapsedTime = 0f, smoothTimer = 0.01f;

            while (elapsedTime < duration)
            {
                elapsedTime += smoothTimer;
                float t = Mathf.Clamp01(elapsedTime / duration);
                float currentTransparency = Mathf.Lerp(0, 1, t);
                _propBlock.SetFloat("_TransparencyRange", currentTransparency);
                MySkin.SetPropertyBlock(_propBlock);
                yield return new WaitForSeconds(smoothTimer);
            }
        }


        public IEnumerator RevelColour()
        {
            float elapsedTime = 0f, smoothTimer = 0.01f;

            while (elapsedTime < _transparencyChangeDuration)
            {
                elapsedTime += smoothTimer;
                float t = Mathf.Clamp01(elapsedTime / _transparencyChangeDuration);
                float currentTransparency = Mathf.Lerp(1, 0, t);
                _propBlock.SetFloat("_HidenRange", currentTransparency);
                MySkin.SetPropertyBlock(_propBlock);
                yield return new WaitForSeconds(smoothTimer);
            }
        }
    }
}