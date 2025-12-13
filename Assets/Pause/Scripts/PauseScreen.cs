using UnityEngine;
using DG.Tweening;
using Core.Events;
using Core.Variables;
using Core.DB.Variables;

namespace Core.Screen
{
    public class PauseScreen : UiScreens
    {
        [SerializeField] DBInt Music, Sound;
        [SerializeField] SOIntegerEvents DestroyStatEvent, ActiveStatEvent, SoundEffectEvent;
        [SerializeField] SOInterger CanPlay, MainMenuStateIndex, GamePlayStateIndex;
        [SerializeField] GameObject MusicOff, SoundOff;

        private void OnEnable()
        {
            UpdateMusicUI();
            UpdateSoundUI();
            OnOpen();
        }
        public void ToggleMusic()
        {
            Music.Value = Music.Value == 1 ? 0 : 1;
            SimpleEventsHolder.UpdateMusicStateEvent?.Invoke();
            UpdateMusicUI();
        }

        public void ToggleSound()
        {
            Sound.Value = Sound.Value == 1 ? 0 : 1;
            SimpleEventsHolder.UpdateSoundStateEvent?.Invoke();
            UpdateSoundUI();
        }

        void UpdateMusicUI()
        {
            MusicOff.SetActive(Music.Value != 1);
        }

        void UpdateSoundUI()
        {
            SoundOff.SetActive(Sound.Value != 1);
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
            Body.DOScale(0, _transitionDuration / 2).SetEase(Ease.InBack).OnComplete(() => {
                CanPlay.Value = 1;
                Destroy(gameObject);
            });
        }
    }
}
