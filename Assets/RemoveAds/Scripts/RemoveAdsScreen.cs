using System;
using DG.Tweening;
using UnityEngine;
using Core.Events;
using Core.DB.Variables;

namespace Core.Screen
{
    public class RemoveAdsScreen : UiScreens
    {
        [SerializeField] GameObject RvBtn, TimerObj;

        void OnEnable()
        {
            SimpleEventsHolder.AdsBlockerEvent += BlockAdForTime;
            if (DBVariablesHolder.AdBlocked.Value == 1)
            {
                RvBtn.SetActive(false);
                TimerObj.SetActive(true);
            }
            else
            {
                RvBtn.SetActive(true);
                TimerObj.SetActive(false);
            }
            OnOpen();
        }

        void OnDisable()
        {
            SimpleEventsHolder.AdsBlockerEvent -= BlockAdForTime;
        }

        void BlockAdForTime()
        {
            DBVariablesHolder.AdBlocked.Value = 1;
            DBVariablesHolder.AdBlockingTime.Value = DateTime.Now.ToString();
            RvBtn.SetActive(false);
            TimerObj.SetActive(true);
            OnClose();
        }

        public override void OnOpen()
        {
            SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(3);
            Body.DOAnchorPosX(0, _transitionDuration).SetEase(Ease.OutBack);
        }

        public override void OnClose()
        {
            SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(2);
            Body.DOAnchorPosX(1500, _transitionDuration/2).SetEase(Ease.InBack).OnComplete(() => gameObject.SetActive(false));
        }
    }
}
