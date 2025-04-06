using UnityEngine;
using DG.Tweening;
using Core.Events;

namespace Core.Screen
{
    public class StoreScreen : MonoBehaviour
    {
        [SerializeField] SOIntegerEvents SoundEffectEvent;
        [SerializeField] RectTransform Body;

        float _tweenTime = 1f;

        private void OnEnable()
        {
            SoundEffectEvent.InvokeSOEvent(3);
            Body.DOAnchorPosX(0, _tweenTime).SetEase(Ease.OutBack);
        }

        public void OnClose()
        {
            SoundEffectEvent.InvokeSOEvent(3);
            Body.DOAnchorPosX(1500, _tweenTime).SetEase(Ease.InBack).OnComplete(()=> gameObject.SetActive(false));
        }
    }
}
