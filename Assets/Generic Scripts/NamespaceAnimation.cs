using UnityEngine;

namespace Core.Animations
{ 

}

namespace Core.Animations.LT
{
    public class SOLeanTween : ScriptableObject
    {
        public GameObject TargetObj { get; set; }

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
            if (TargetObj != null)
                LeanTween.cancel(TargetObj);
        }
    }
}

