using UnityEngine;
using DG.Tweening;
using Core.Events;
using Core.States;
using Core.GamePlay;
using Core.DB.Variables;

namespace Core.Screen
{
    public class PauseScreen : UiScreens
    {
        [SerializeField] GameObject MusicOff, SoundOff;

        private void OnEnable()
        {
            UpdateMusicUI();
            UpdateSoundUI();
            OnOpen();
        }
        public void ToggleMusic()
        {
            DBVariablesHolder.Music.Value = DBVariablesHolder.Music.Value == 1 ? 0 : 1;
            SimpleEventsHolder.UpdateMusicStateEvent?.Invoke();
            UpdateMusicUI();
        }

        public void ToggleSound()
        {
            DBVariablesHolder.Sound.Value = DBVariablesHolder.Sound.Value == 1 ? 0 : 1;
            SimpleEventsHolder.UpdateSoundStateEvent?.Invoke();
            UpdateSoundUI();
        }

        void UpdateMusicUI()
        {
            MusicOff.SetActive(DBVariablesHolder.Music.Value != 1);
        }

        void UpdateSoundUI()
        {
            SoundOff.SetActive(DBVariablesHolder.Sound.Value != 1);
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
            Body.DOScale(0, _transitionDuration / 2).SetEase(Ease.InBack).OnComplete(() => {
                LevelsManager.I.CanPlay = true;
                Destroy(gameObject);
            });
        }
    }
}
