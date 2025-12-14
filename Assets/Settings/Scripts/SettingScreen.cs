using DG.Tweening;
using UnityEngine;
using Core.Events;
using Core.DB.Variables;

namespace Core.Screen
{
    public class SettingScreen : UiScreens
    {
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
            SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(2);
            MusicBtn.DOAnchorPosY(-150, _transitionDuration).SetEase(Ease.OutBack);
            SoundBtn.DOAnchorPosY(-240, _transitionDuration).SetEase(Ease.OutBack);
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

        public override void OnClose()
        {
            MusicBtn.DOAnchorPosY(-60, _transitionDuration).SetEase(Ease.InBack);
            SoundBtn.DOAnchorPosY(-60, _transitionDuration).SetEase(Ease.InBack).OnComplete(()=>gameObject.SetActive(false));
            SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(2);
        }
    }
}
