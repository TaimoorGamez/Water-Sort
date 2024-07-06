using UnityEngine;

namespace Core.Animations
{ 

}

namespace Core.Animations.LT
{
    public class SOLeanTween : ScriptableObject
    {
        [SerializeField] public GameObject TargetObj;
        [SerializeField] protected Vector3 TargetAction;
        [SerializeField] protected float Duration;
        [SerializeField] protected LeanTweenType CurrentEaseType = LeanTweenType.linear;
        [SerializeField] bool IsLoop = false, StateAfterComplete = true;

        protected LTDescr ltAnimation;

        public virtual void PlayAnimation()
        {
            ltAnimation.setEase(CurrentEaseType);

            if (IsLoop)
            {
                ltAnimation.setLoopClamp();
            }

            ltAnimation.setOnComplete(() => TargetObj.SetActive(StateAfterComplete));
        }

        public virtual void CancleAnimation()
        {
            if (ltAnimation != null)
            {
                LeanTween.cancel(ltAnimation.id); // Cancel by ID
                ltAnimation = null; // Optionally set to null to avoid accidental reuse
            }
        }
    }
}

