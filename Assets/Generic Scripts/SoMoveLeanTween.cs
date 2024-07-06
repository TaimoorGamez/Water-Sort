using UnityEngine;


namespace Core.Animations.LT
{
    [CreateAssetMenu(fileName = "MoveLT", menuName = "ScriptableObjects/Animations/LT/Move")]
    public class SOMoveLeanTween : SOLeanTween
    {
        public override void PlayAnimation()
        {
            ltAnimation = LeanTween.moveLocal(TargetObj, TargetAction, Duration);
            base.PlayAnimation();
        }

        public override void CancleAnimation()
        {
            base.CancleAnimation();
        }
    }
}
