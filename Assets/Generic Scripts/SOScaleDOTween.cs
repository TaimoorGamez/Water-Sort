using DG.Tweening;
using UnityEngine;

namespace Core.Animations.DT
{
    [CreateAssetMenu(fileName = "Scale", menuName = "ScriptableObjects/Animations/Scale")]
    public class SOScaleDOTween : SODOTween
    {
        public override void PlayAnimation()
        {
            _tween = TargetObj.transform.DOScale(TargetAction, Duration);
            base.PlayAnimation();
        }

        public override void ReseToDefault()
        {
            TargetObj.transform.localScale = OriginalVallue;
        }
    }
}
