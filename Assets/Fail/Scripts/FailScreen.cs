using UnityEngine;
using DG.Tweening;
using Core.Events;
using Core.Variables;

namespace Core.Screen
{
    public class FailScreen : MonoBehaviour
    {
        [SerializeField] SOIntegerEvents DestroyStatEvent, ActiveStatEvent;
        [SerializeField] SOEvents UpdateMovesEvent, RestartLevelEvent, DestroyLevelEvent;
        [SerializeField] SOInterger TotalMoves, CanPlay, MainMenuStateIndex, GamePlayStateIndex;
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

        public void RestartLevel()
        {
            RestartLevelEvent.InvokeSOEvent();
            ClosePanel();
        }
        public void GoHome()
        {
            DestroyLevelEvent.InvokeSOEvent();
            DestroyStatEvent.InvokeSOEvent(GamePlayStateIndex.Value);
            ActiveStatEvent.InvokeSOEvent(MainMenuStateIndex.Value);
            ClosePanel();
        }

        void ClosePanel()
        {
            Body.DOScale(0, _tweenTime).SetEase(Ease.InBack).OnComplete(() => Destroy(gameObject));
        }
    }
}
