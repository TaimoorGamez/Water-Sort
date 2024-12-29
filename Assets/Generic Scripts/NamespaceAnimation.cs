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

        [SerializeField] protected float Duration;
        [SerializeField] protected Ease CurrentEaseType;
        [SerializeField] bool IsLoop = false, CanChangeStateOnComlete = false, StateOnComplete = true, ResetOnComplete = false;
        [SerializeField] protected Vector3 TargetAction, OriginalVallue;

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
                { TargetObj.SetActive(StateOnComplete); }
                if (ResetOnComplete)
                {
                    ReseToDefault();
                }
                });
        }

        public virtual void CancleAnimation()
        {
            if (TargetObj != null)
                ltAnimation.Kill();
        }

        public virtual void ReseToDefault()
        {

        }
    }
}

