using UnityEngine;

namespace Core.Animations.LT
{
    [CreateAssetMenu(fileName = "Rotate", menuName = "ScriptableObjects/Animations/LT/Rotation")]
    public class SORotateLeanTween : SOLeanTween
    {
        public override void PlayAnimation()
        {
            ltAnimation = LeanTween.rotate(TargetObj, TargetAction, Duration);
            base.PlayAnimation();
        }
    }
}
