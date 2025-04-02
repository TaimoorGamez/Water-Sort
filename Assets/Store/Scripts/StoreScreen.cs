using UnityEngine;
using DG.Tweening;

namespace Core.Screen
{
    public class StoreScreen : MonoBehaviour
    {
        [SerializeField] RectTransform Body;

        float _tweenTime = 1f;

        private void OnEnable()
        {
            Body.DOAnchorPosX(0, _tweenTime).SetEase(Ease.OutBack);
        }

        public void OnClose()
        {
            Body.DOAnchorPosX(500, _tweenTime/2).SetEase(Ease.OutBack).OnComplete(()=> gameObject.SetActive(false));
        }
    }
}
