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
        [SerializeField] Transform Body;
        [SerializeField] GameObject MusicOff, SoundOff;

        float _tweenTime = 0.25f;

        private void OnEnable()
        {
            UpdateMusicStateEvent.EventHandler += UpdateMusicState;
            UpdateSoundStateEvent.EventHandler += UpdateSoundState;
            SoundEffectEvent.InvokeSOEvent(2);
            Body.DOScale(1, _tweenTime).SetEase(Ease.OutBack);
            UpdateMusicState();
            UpdateSoundState();
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

        public override void OnClose()
        {
            Body.DOScale(0, _tweenTime).SetEase(Ease.InBack).OnComplete(() => {
                CanPlay.Value = 1;
                Destroy(gameObject);
            });
            SoundEffectEvent.InvokeSOEvent(2);
        }

        public void GoHome()
        {
            DestroyLevelEvent.InvokeSOEvent();
            DestroyStatEvent.InvokeSOEvent(GamePlayStateIndex.Value);
            ActiveStatEvent.InvokeSOEvent(MainMenuStateIndex.Value);
            OnClose();
        }
    }
}
