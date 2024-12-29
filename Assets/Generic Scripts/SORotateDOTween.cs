using DG.Tweening;
using UnityEngine;

namespace Core.Animations.DT
{
    [CreateAssetMenu(fileName = "RotateLT", menuName = "ScriptableObjects/Animations/LT/Rotation")]
    public class SORotateDOTween : SODOTween
    {
        public override void PlayAnimation()
        {
            ltAnimation = TargetObj.transform.DORotate(TargetAction, Duration);
            base.PlayAnimation();
        }
        public override void ReseToDefault()
        {
            TargetObj.transform.eulerAngles = OriginalVallue;
        }
    }
}
