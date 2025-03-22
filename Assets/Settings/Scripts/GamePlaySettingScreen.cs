using DG.Tweening;
using UnityEngine;
using Core.Events;
using Core.Variables;
using Core.DB.Variables;

namespace Core.Screen
{
    public class GamePlaySettingScreen : MonoBehaviour
    {
        [SerializeField] DBInt Music, Sound;
        [SerializeField] SOEvents UpdateMusicStateEvent, UpdateSoundStateEvent;
        [SerializeField] SOIntegerEvents DestroyStateEvent;
        [SerializeField] SOInterger SettingStateIndex;
        [SerializeField] RectTransform MusicBtn, SoundBtn;
        [SerializeField] GameObject MusicOn, MusicOff, SoundOn, SoundOff;

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

        public void OnCloseSetting()
        {
            MusicBtn.DOAnchorPosY(-60, _tweenTime).SetEase(Ease.InBack);
            SoundBtn.DOAnchorPosY(-60, _tweenTime).SetEase(Ease.InBack).OnComplete(()=>DestroyStateEvent.InvokeSOEvent(SettingStateIndex.Value));
        }
    }
}
