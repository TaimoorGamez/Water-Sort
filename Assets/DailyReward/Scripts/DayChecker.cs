using TMPro;
using System;
using UnityEngine;
using Core.Variables;
using Core.DB.Variables;
using System.Collections;

namespace ProjectCore.DailyReward
{
    public class DayChecker : MonoBehaviour
    {
        [SerializeField] DBString LastDate;
        [SerializeField] DBInt ToDay;
        [SerializeField] GameObject DailyRewardView;
        [SerializeField] TextMeshProUGUI RemainingTimeText;

        Coroutine _timerRotine;

        private void Start()
        {
            ChangeDay();
        }

        public void ChangeDay()
        {
            if(_timerRotine != null)
            StopCoroutine(_timerRotine);

            DateTime currentDate = DateTime.Now;
            DateTime lastRewardDate = DateTime.Parse(LastDate.Value);
            int daysSinceLastReward = (currentDate - lastRewardDate).Days;
            int daysGreater = daysSinceLastReward / 1;
            Debug.Log(currentDate);

            if (daysGreater >= 1)
            {
                if (daysGreater == 1 && ToDay.Value < 7)
                {
                    ToDay.Value = (ToDay.Value + 1);
                }
                else
                {
                    ToDay.Value = 1;
                }
                LastDate.Value = (DateTime.Now.ToString()); 
                DailyRewardView.SetActive(true);
            }
            else
            {
                DateTime nextRewardDate = DateTime.Parse(LastDate.Value).AddDays(1);
                TimeSpan remainingTime = nextRewardDate - DateTime.Now;
                float secondsRemaining = (float)remainingTime.TotalSeconds;
                _timerRotine = StartCoroutine(UpdateRemainingTime(secondsRemaining));
            }
        }

        private IEnumerator UpdateRemainingTime(float seconds)
        {
            WaitForSeconds duration = new WaitForSeconds(1);
            while (seconds > 1)
            {
                RemainingTimeText.text = string.Format("{0:00}:{1:00}:{2:00}", Mathf.Floor(seconds / 3600), Mathf.Floor((seconds % 3600) / 60), Mathf.Floor(seconds % 60));
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
