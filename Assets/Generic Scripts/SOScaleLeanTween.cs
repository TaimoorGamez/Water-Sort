using UnityEngine;

namespace Core.Animations.LT
{
    [CreateAssetMenu(fileName = "Scale", menuName = "ScriptableObjects/Animations/LT/Scale")]
    public class SOScaleLeanTween : SOLeanTween
    {
        public override void PlayAnimation()
        {
            ltAnimation = LeanTween.scale(TargetObj, TargetAction, Duration);
            base.PlayAnimation();
        }
    }
}
