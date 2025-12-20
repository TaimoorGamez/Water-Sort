using DG.Tweening;
using UnityEngine;

namespace Core.Animations.DT
{
    [CreateAssetMenu(fileName = "Anchor", menuName = "ScriptableObjects/Animations/SOAnchor")]
    public class SOAnchorMove : SODOTween
    {
        RectTransform _objRect;

        public override void PlayAnimation()
        {
            _objRect = TargetObj.GetComponent<RectTransform>();
            if (_objRect == null)
                return;

            _tween = _objRect.DOAnchorPos(TargetAction, Duration).SetTarget(TargetObj).SetLink(TargetObj, LinkBehaviour.KillOnDestroy);
            base.PlayAnimation();
        }

        public override void ReseToDefault()
        {
            _objRect.anchoredPosition = OriginalVallue;
        }
    }
}
