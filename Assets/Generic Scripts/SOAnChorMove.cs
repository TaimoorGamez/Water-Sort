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
            _tween = _objRect.DOAnchorPos(TargetAction, Duration);
            base.PlayAnimation();
        }

        public override void ReseToDefault()
        {
            _objRect.anchoredPosition = OriginalVallue;
        }
    }
}
