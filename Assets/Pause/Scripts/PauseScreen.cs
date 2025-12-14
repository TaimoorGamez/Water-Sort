using UnityEngine;
using DG.Tweening;
using Core.Events;
using Core.States;
using Core.DB.Variables;
using Core.GamePlay.WaterSort;

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
            DBIntsHolder.Music.Value = DBIntsHolder.Music.Value == 1 ? 0 : 1;
            SimpleEventsHolder.UpdateMusicStateEvent?.Invoke();
            UpdateMusicUI();
        }

        public void ToggleSound()
        {
            DBIntsHolder.Sound.Value = DBIntsHolder.Sound.Value == 1 ? 0 : 1;
            SimpleEventsHolder.UpdateSoundStateEvent?.Invoke();
            UpdateSoundUI();
        }

        void UpdateMusicUI()
        {
            MusicOff.SetActive(DBIntsHolder.Music.Value != 1);
        }

        void UpdateSoundUI()
        {
            SoundOff.SetActive(DBIntsHolder.Sound.Value != 1);
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
