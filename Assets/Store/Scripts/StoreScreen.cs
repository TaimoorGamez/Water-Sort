using UnityEngine;
using DG.Tweening;
using Core.Events;

namespace Core.Screen
{
    public class StoreScreen : UiScreens
    {
        [SerializeField] SOIntegerEvents SoundEffectEvent;

        private void OnEnable()
        {
            OnOpen();
        }

        public override void OnOpen()
        {
            SoundEffectEvent.InvokeSOEvent(3);
            Body.DOAnchorPosX(0, _transitionDuration).SetEase(Ease.OutBack);
        }

        public override void OnClose()
        {
            SoundEffectEvent.InvokeSOEvent(2);
            Body.DOAnchorPosX(1500, _transitionDuration/2).SetEase(Ease.InBack).OnComplete(()=> gameObject.SetActive(false));
        }
    }
}
