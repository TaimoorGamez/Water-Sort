using DG.Tweening;
using UnityEngine;

namespace Core.Animations.DT
{
    [CreateAssetMenu(fileName = "Scale", menuName = "ScriptableObjects/Animations/LT/Scale")]
    public class SOScaleLeanTween : SOLeanTween
    {
        public override void PlayAnimation()
        {
            ltAnimation = TargetObj.transform.DOScale(TargetAction, Duration);
            base.PlayAnimation();
        }

        public override void CancleAnimation()
        {
            base.CancleAnimation();
        }
    }
}
