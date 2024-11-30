using DG.Tweening;
using UnityEngine;

namespace Core.Animations.DT
{
    [CreateAssetMenu(fileName = "MoveLT", menuName = "ScriptableObjects/Animations/LT/Move")]
    public class SOMoveLeanTween : SOLeanTween
    {
        public override void PlayAnimation()
        {
            ltAnimation = TargetObj.transform.DOLocalMove(TargetAction, Duration);
            base.PlayAnimation();
        }

        public override void CancleAnimation()
        {
            base.CancleAnimation();
        }
    }
}
