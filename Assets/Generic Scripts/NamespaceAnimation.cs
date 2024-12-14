using UnityEngine;
using DG.Tweening;

namespace Core.Animations
{ 

}

namespace Core.Animations.DT
{
    public class SODOTween : ScriptableObject
    {
        public GameObject TargetObj { get; set; }

        [SerializeField] protected Vector3 TargetAction;
        [SerializeField] protected float Duration;
        [SerializeField] protected Ease CurrentEaseType;
        [SerializeField] bool IsLoop = false, CanChangeStateOnComlete = false, StateOnComplete = true;

        protected Tween ltAnimation;

        public virtual void PlayAnimation()
        {
            ltAnimation.SetEase(CurrentEaseType);

            if (IsLoop)
            {
                ltAnimation.Loops();
            }

            ltAnimation.OnComplete(() => {
                if (CanChangeStateOnComlete)
                    TargetObj.SetActive(StateOnComplete);
                });
        }

        public virtual void CancleAnimation()
        {
            if (TargetObj != null)
                ltAnimation.Kill();
        }
    }
}

