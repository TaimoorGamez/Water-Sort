using System;
using DG.Tweening;
using UnityEngine;
using Core.Events;
using Core.Variables;
using Core.DB.Variables;

namespace Core.Screen
{
    public class RemoveAdsScreen : MonoBehaviour
    {
        [SerializeField] SOEvents AdsBlockerEvent;
        [SerializeField] DBInt AdBlocked;
        [SerializeField] DBString AdBlockingTime;
        [SerializeField] RectTransform Body;
        [SerializeField] GameObject RvBtn, TimerObj;

        float _durationTweeing = 0.5f, reseTime = 900;

        void OnEnable()
        {
            AdsBlockerEvent.EventHandler += BlockAdForTime;
            Body.DOAnchorPosX(0, _durationTweeing).SetEase(Ease.OutBack);
            if (AdBlocked.Value == 1)
            {
                RvBtn.SetActive(false);
                TimerObj.SetActive(true);
            }
            else
            {
                RvBtn.SetActive(true);
                TimerObj.SetActive(false);
            }
        }

        void OnDisable()
        {
            AdsBlockerEvent.EventHandler -= BlockAdForTime;
        }

        void BlockAdForTime()
        {
            AdBlocked.Value = 1;
            AdBlockingTime.Value = DateTime.Now.ToString();
            RvBtn.SetActive(false);
            TimerObj.SetActive(true);
            OnClose();
        }

        public void OnClose()
        {
            Body.DOAnchorPosX(1500, _durationTweeing).SetEase(Ease.InBack).OnComplete(() => gameObject.SetActive(false));
        }
    }
}
