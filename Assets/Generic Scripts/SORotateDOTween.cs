using DG.Tweening;
using UnityEngine;

namespace Core.Animations.DT
{
    [CreateAssetMenu(fileName = "RotateLT", menuName = "ScriptableObjects/Animations/Rotation")]
    public class SORotateDOTween : SODOTween
    {
        public override void PlayAnimation()
        {
            _tween = TargetObj.transform.DORotate(TargetAction, Duration);
            base.PlayAnimation();
        }
        public override void ReseToDefault()
        {
            TargetObj.transform.eulerAngles = OriginalVallue;
        }
    }
}
