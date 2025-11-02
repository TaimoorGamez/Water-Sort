using DG.Tweening;
using UnityEngine;
using Core.Events;
using Core.Variables;
using Core.DB.Variables;

namespace Core.Screen
{
    public class SettingScreen : UiScreens
    {
        [SerializeField] DBInt Music, Sound;
        [SerializeField] SOEvents UpdateMusicStateEvent, UpdateSoundStateEvent;
        [SerializeField] SOIntegerEvents DestroyStateEvent, SoundEffectEvent;
        [SerializeField] SOInterger SettingStateIndex;
        [SerializeField] RectTransform MusicBtn, SoundBtn;
        [SerializeField] GameObject MusicOff, SoundOff;

        float _tweenTime = 0.5f;

        private void OnEnable()
        {
            UpdateMusicStateEvent.EventHandler += UpdateMusicState;
            UpdateSoundStateEvent.EventHandler += UpdateSoundState;
        }

        private void Start()
        {
            UpdateMusicState();
            UpdateSoundState();
            MusicBtn.DOAnchorPosY(-150, _tweenTime).SetEase(Ease.OutBack);
            SoundBtn.DOAnchorPosY(-240, _tweenTime).SetEase(Ease.OutBack);
            SoundEffectEvent.InvokeSOEvent(2);
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

        public override void OnClose()
        {
            MusicBtn.DOAnchorPosY(-60, _tweenTime).SetEase(Ease.InBack);
            SoundBtn.DOAnchorPosY(-60, _tweenTime).SetEase(Ease.InBack).OnComplete(()=>DestroyStateEvent.InvokeSOEvent(SettingStateIndex.Value));
            SoundEffectEvent.InvokeSOEvent(2);
        }
    }
}
