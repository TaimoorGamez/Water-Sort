using Core.Events;
using Core.States;
using DG.Tweening;
using Core.GamePlay;

namespace Core.Screen
{
    public class FailScreen : UiScreens
    {
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
            LevelsManager.I.TotalMoves += _extraMoves;
            SimpleEventsHolder.UpdateMovesEvent?.Invoke();
            LevelsManager.I.CanPlay = true;
        }

        public void RestartLevel()
        {
            SimpleEventsHolder.RestartLevelEvent?.Invoke();
            OnClose();
        }
        public void GoHome()
        {
            SimpleEventsHolder.DestroyLevelEvent?.Invoke();
            SingleIntegerEventsHolder.DestroyStatEvent?.Invoke(StateManager.I.GamePlayStateIndex);
            SingleIntegerEventsHolder.ActiveStateEvent?.Invoke(StateManager.I.MainMenuStateIndex);
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
