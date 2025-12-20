using DG.Tweening;
using UnityEngine;

namespace Core.Animations.DT
{
    [CreateAssetMenu(fileName = "MoveLT", menuName = "ScriptableObjects/Animations/Move")]
    public class SOMoveDOTween : SODOTween
    {
        public override void PlayAnimation()
        {
            _tween = TargetObj.transform.DOLocalMove(TargetAction, Duration).SetTarget(TargetObj).SetLink(TargetObj, LinkBehaviour.KillOnDestroy);
            base.PlayAnimation();
        }

        public override void ReseToDefault()
        {
            TargetObj.transform.localPosition = OriginalVallue;
        }
    }
}
