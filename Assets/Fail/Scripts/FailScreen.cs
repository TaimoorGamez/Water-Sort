using UnityEngine;
using DG.Tweening;
using Core.Events;
using Core.Variables;

namespace Core.Screen
{
    public class FailScreen : UiScreens
    {
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
            SingleIntegerEventsHolder.DestroyStatEvent?.Invoke(GamePlayStateIndex.Value);
            SingleIntegerEventsHolder.ActiveStateEvent?.Invoke(MainMenuStateIndex.Value);
            OnClose();
        }

        public override void OnOpen()
        {
            SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(3);
            Body.DOScale(1, _transitionDuration).SetEase(Ease.OutBack);
        }

        public override void OnClose()
        {
            SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(2);
            Body.DOScale(0, _transitionDuration/2).SetEase(Ease.InBack).OnComplete(() => Destroy(gameObject));
        }
    }
}
