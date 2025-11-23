using UnityEngine;
using DG.Tweening;
using Core.Events;
using Core.Variables;

namespace Core.Screen
{
    public class FailScreen : UiScreens
    {
        [SerializeField] SOIntegerEvents DestroyStatEvent, ActiveStatEvent, SoundEffectEvent;
        [SerializeField] SOEvents UpdateMovesEvent, RestartLevelEvent, DestroyLevelEvent, MoreMovesEvent;
        [SerializeField] SOInterger TotalMoves, CanPlay, MainMenuStateIndex, GamePlayStateIndex;

        int _extraMoves = 10;

        private void OnEnable()
        {
            MoreMovesEvent.EventHandler += AddMoreMoves;
            OnOpen();
        }

        private void OnDisable()
        {
            MoreMovesEvent.EventHandler -= AddMoreMoves;
        }

        void AddMoreMoves()
        {
            OnClose();
            TotalMoves.Value += _extraMoves;
            UpdateMovesEvent.InvokeSOEvent();
            CanPlay.Value = 1;
        }

        public void RestartLevel()
        {
            RestartLevelEvent.InvokeSOEvent();
            OnClose();
        }
        public void GoHome()
        {
            DestroyLevelEvent.InvokeSOEvent();
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
