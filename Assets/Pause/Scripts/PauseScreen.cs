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
        [SerializeField] SOEvents RestartLevelEvent, DestroyLevelEvent, UpdateMusicStateEvent, UpdateSoundStateEvent;
        [SerializeField] SOInterger CanPlay, MainMenuStateIndex, GamePlayStateIndex;
        [SerializeField] GameObject MusicOff, SoundOff;

        private void OnEnable()
        {
            UpdateMusicStateEvent.EventHandler += UpdateMusicState;
            UpdateSoundStateEvent.EventHandler += UpdateSoundState;
            UpdateMusicState();
            UpdateSoundState();
            OnOpen();
        }

        private void OnDisable()
        {
            UpdateMusicStateEvent.EventHandler -= UpdateMusicState;
            UpdateSoundStateEvent.EventHandler -= UpdateSoundState;
        }

        void UpdateMusicState()
        {
            MusicOff.SetActive(Music.Value != 1);
        }

        void UpdateSoundState()
        {
            SoundOff.SetActive(Sound.Value != 1);
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
            Body.DOScale(0, _transitionDuration / 2).SetEase(Ease.InBack).OnComplete(() => {
                CanPlay.Value = 1;
                Destroy(gameObject);
            });
        }
    }
}
