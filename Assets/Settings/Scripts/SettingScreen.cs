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
        [SerializeField] SOIntegerEvents SoundEffectEvent;
        [SerializeField] SOInterger SettingStateIndex;
        [SerializeField] RectTransform MusicBtn, SoundBtn;
        [SerializeField] GameObject MusicOff, SoundOff;

        private void OnEnable()
        {
            OnOpen();
        }

        public override void OnOpen()
        {
            UpdateMusicUI();
            UpdateSoundUI();
            SoundEffectEvent.InvokeSOEvent(2);
            MusicBtn.DOAnchorPosY(-150, _transitionDuration).SetEase(Ease.OutBack);
            SoundBtn.DOAnchorPosY(-240, _transitionDuration).SetEase(Ease.OutBack);
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

        public override void OnClose()
        {
            MusicBtn.DOAnchorPosY(-60, _transitionDuration).SetEase(Ease.InBack);
            SoundBtn.DOAnchorPosY(-60, _transitionDuration).SetEase(Ease.InBack).OnComplete(()=>gameObject.SetActive(false));
            SoundEffectEvent.InvokeSOEvent(2);
        }
    }
}
