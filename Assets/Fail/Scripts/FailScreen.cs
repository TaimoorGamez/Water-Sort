using UnityEngine;
using DG.Tweening;
using Core.Events;
using Core.Variables;

namespace Core.Screen
{
    public class FailScreen : MonoBehaviour
    {
        [SerializeField] SOEvents UpdateMovesEvent;
        [SerializeField] SOInterger TotalMoves, CanPlay;
        [SerializeField] Transform Body;

        int _extraMoves = 10;
        float _tweenTime = 0.25f;

        private void OnEnable()
        {
            Body.DOScale(1, _tweenTime).SetEase(Ease.OutBack);
        }


        public void AddMoreMoves()
        {
            ClosePanel();
            TotalMoves.Value += _extraMoves;
            UpdateMovesEvent.InvokeSOEvent();
            CanPlay.Value = 1;
        }

        void ClosePanel()
        {
            Body.DOScale(0, _tweenTime).SetEase(Ease.InBack).OnComplete(() => Destroy(gameObject));
        }
    }
}
