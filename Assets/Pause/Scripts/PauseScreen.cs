using UnityEngine;
using DG.Tweening;
using Core.Events;
using Core.Variables;
using Core.DB.Variables;

namespace Core.Screen
{
    public class PauseScreen : MonoBehaviour
    {
        [SerializeField] DBInt Music, Sound;
        [SerializeField] SOIntegerEvents DestroyStatEvent, ActiveStatEvent, SoundEffectEvent;
        [SerializeField] SOEvents RestartLevelEvent, DestroyLevelEvent, UpdateMusicStateEvent, UpdateSoundStateEvent;
        [SerializeField] SOInterger CanPlay, MainMenuStateIndex, GamePlayStateIndex;
        [SerializeField] Transform Body;
        [SerializeField] GameObject MusicOn, MusicOff, SoundOn, SoundOff;

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
            MusicOn.SetActive(Music.Value == 1);
            MusicOff.SetActive(Music.Value != 1);
        }

        void UpdateSoundState()
        {
            SoundOn.SetActive(Sound.Value == 1);
            SoundOff.SetActive(Sound.Value != 1);
        }

        public void RestartLevel()
        {
            RestartLevelEvent.InvokeSOEvent();
            ClosePanel();
        }

        public void ClosePanel()
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
            ClosePanel();
        }
    }
}
