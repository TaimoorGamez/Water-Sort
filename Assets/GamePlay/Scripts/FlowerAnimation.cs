using UnityEngine;
using DG.Tweening;

namespace Core.GamePlay.WaterSort
{
    public class FlowerAnimation : CapAnimation
    {
        [SerializeField] Renderer MySkin;
        [SerializeField] Texture MyTexture;

        float _transparencyChangeDuration = 2.5f, _targetTransparency = 1;
        MaterialPropertyBlock _propBlock;


        public override void PlayCapAnimation(Color currentColor)
        {
            Debug.Log("line 17");
            _propBlock = new MaterialPropertyBlock();
            _propBlock.SetColor("_BaseColor", currentColor);
            _propBlock.SetTexture("_MainTex", MyTexture);
            _propBlock.SetFloat("_ColorRange", 0);
            MySkin.SetPropertyBlock(_propBlock); transform.DOLocalMoveY(0,0.2f).OnComplete(() =>
            {
                Debug.Log("line 24");
                // Animate the _ColorRange property
                DOTween.To(() => 0f, value =>
                {
                    Debug.Log("line 28");
                    _propBlock.SetFloat("_ColorRange", value);
                    MySkin.SetPropertyBlock(_propBlock);
                }, _targetTransparency, _transparencyChangeDuration);
            });
        }

    }
}
