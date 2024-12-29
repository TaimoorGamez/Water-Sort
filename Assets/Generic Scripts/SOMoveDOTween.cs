using DG.Tweening;
using UnityEngine;

namespace Core.Animations.DT
{
    [CreateAssetMenu(fileName = "MoveLT", menuName = "ScriptableObjects/Animations/LT/Move")]
    public class SOMoveDOTween : SODOTween
    {
        public override void PlayAnimation()
        {
            ltAnimation = TargetObj.transform.DOLocalMove(TargetAction, Duration);
            base.PlayAnimation();
        }

        public override void ReseToDefault()
        {
            TargetObj.transform.localPosition = OriginalVallue;
        }
    }
}
