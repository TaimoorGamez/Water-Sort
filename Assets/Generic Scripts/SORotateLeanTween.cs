using UnityEngine;

namespace Core.Animations.LT
{
    [CreateAssetMenu(fileName = "RotateLT", menuName = "ScriptableObjects/Animations/LT/Rotation")]
    public class SORotateLeanTween : SOLeanTween
    {
        public override void PlayAnimation()
        {
            ltAnimation = LeanTween.rotate(TargetObj, TargetAction, Duration);
            base.PlayAnimation();
        }
        public override void CancleAnimation()
        {
            base.CancleAnimation();
        }
    }
}
