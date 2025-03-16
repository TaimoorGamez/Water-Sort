using UnityEngine;
using DG.Tweening;
using Core.Events;
using Core.Variables;

namespace Core.Screen
{
    public class PauseScreen : MonoBehaviour
    {
        [SerializeField] SOEvents RestartLevelEvent;
        [SerializeField] SOInterger CanPlay;
        [SerializeField] Transform Body;

        float _tweenTime = 0.25f;

        private void OnEnable()
        {
            Body.DOScale(1, _tweenTime).SetEase(Ease.OutBack);
        }

        public void RestartLevel()
        {
            ClosePanel();
            RestartLevelEvent.InvokeSOEvent();
        }

        public void ClosePanel()
        {
            Body.DOScale(0, _tweenTime).SetEase(Ease.InBack).OnComplete(() => Destroy(gameObject));
        }
    }
}
