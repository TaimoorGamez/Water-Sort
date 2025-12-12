using UnityEngine;
using DG.Tweening;
using Core.Events;
using Core.Variables;

namespace Core.Screen
{
    public class FailScreen : UiScreens
    {
        [SerializeField] SOIntegerEvents DestroyStatEvent, ActiveStatEvent, SoundEffectEvent;
        [SerializeField] SOInterger TotalMoves, CanPlay, MainMenuStateIndex, GamePlayStateIndex;

        int _extraMoves = 10;

        private void OnEnable()
        {
            SimpleEventsHolder.MoreMovesEvent += AddMoreMoves;
            OnOpen();
        }

        private void OnDisable()
        {
            SimpleEventsHolder.MoreMovesEvent -= AddMoreMoves;
        }

        void AddMoreMoves()
        {
            OnClose();
            TotalMoves.Value += _extraMoves;
            SimpleEventsHolder.UpdateMovesEvent?.Invoke();
            CanPlay.Value = 1;
        }

        public void RestartLevel()
        {
            SimpleEventsHolder.RestartLevelEvent?.Invoke();
            OnClose();
        }
        public void GoHome()
        {
            SimpleEventsHolder.DestroyLevelEvent?.Invoke();
            DestroyStatEvent.InvokeSOEvent(GamePlayStateIndex.Value);
            ActiveStatEvent.InvokeSOEvent(MainMenuStateIndex.Value);
            OnClose();
        }

        public override void OnOpen()
        {
            SoundEffectEvent.InvokeSOEvent(3);
            Body.DOScale(1, _transitionDuration).SetEase(Ease.OutBack);
        }

        public override void OnClose()
        {
            SoundEffectEvent.InvokeSOEvent(2);
            Body.DOScale(0, _transitionDuration/2).SetEase(Ease.InBack).OnComplete(() => Destroy(gameObject));
        }
    }
}
