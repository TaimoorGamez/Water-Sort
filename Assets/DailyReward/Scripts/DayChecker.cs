using TMPro;
using System;
using UnityEngine;
using Core.Events;
using Core.DB.Variables;
using System.Collections;

namespace Core.DailyReward
{
    public class DayChecker : MonoBehaviour
    {
        [SerializeField] DBString LastDate;
        [SerializeField] DBInt ToDay, RewardClaimed;
        [SerializeField] SOEvents UpDateState;
        [SerializeField] GameObject DailyRewardView, NotificationIcon;
        [SerializeField] TextMeshProUGUI RemainingTimeText, PanelTimer;

        Coroutine _timerRotine;

        private void OnEnable()
        {
            UpDateState.EventHandler += UpdateDate;
        }

        private void OnDisable()
        {
            UpDateState.EventHandler -= UpdateDate;
            
            if(_timerRotine != null)
            StopCoroutine(_timerRotine);
        }

        private void Start()
        {
            ChangeDay();
        }

        public void ChangeDay()
        {
            if(_timerRotine != null)
            StopCoroutine(_timerRotine);

            NotificationIcon.SetActive(false);
            RemainingTimeText.gameObject.SetActive(false);
            DateTime currentDate = DateTime.Now;
            DateTime lastRewardDate = DateTime.Parse(LastDate.Value);
            int daysSinceLastReward = (currentDate - lastRewardDate).Days;
            int daysGreater = daysSinceLastReward / 1;

            if (daysGreater >= 1)
            {
                RewardClaimed.Value = 0;
                NotificationIcon.SetActive(true);
                if (daysGreater == 1 && ToDay.Value < 7)
                {
                    ToDay.Value = (ToDay.Value + 1);
                }
                else
                {
                    ToDay.Value = 1;
                }
                DailyRewardView.SetActive(true);
            }
            else
            {
                RemainingTimeText.gameObject.SetActive(true);
                DateTime nextRewardDate = DateTime.Parse(LastDate.Value).AddDays(1);
                TimeSpan remainingTime = nextRewardDate - DateTime.Now;
                float secondsRemaining = (float)remainingTime.TotalSeconds;
                _timerRotine = StartCoroutine(UpdateRemainingTime(secondsRemaining));
            }
        }

        void UpdateDate()
        {
            LastDate.Value = (DateTime.Now.ToString()); 
            ChangeDay();
        }

        private IEnumerator UpdateRemainingTime(float seconds)
        {
            WaitForSeconds duration = new WaitForSeconds(1);
            while (seconds > 1)
            {
                RemainingTimeText.text = string.Format("{0:00}:{1:00}:{2:00}", Mathf.Floor(seconds / 3600), Mathf.Floor((seconds % 3600) / 60), Mathf.Floor(seconds % 60));
                PanelTimer.text = string.Format("{0:00}:{1:00}:{2:00}", Mathf.Floor(seconds / 3600), Mathf.Floor((seconds % 3600) / 60), Mathf.Floor(seconds % 60));
                seconds--;
                yield return duration;
            }

            if (seconds < 1)
            {
                ChangeDay();
            }
        }
    }

}
