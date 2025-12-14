using TMPro;
using System;
using UnityEngine;
using Core.DB.Variables;
using System.Collections;

namespace Core.Screen
{
    public class AdBlockingHandler : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI RemainingTimeText, PanelTimer;

        float _blockedTime = 15 * 60;
        Coroutine _timerRotine;

        void OnEnable()
        {
            if (DBVariablesHolder.AdBlocked.Value == 1)
            {
                DateTime lastDate = DateTime.Parse(DBVariablesHolder.AdBlockingTime.Value);
                TimeSpan passedTime = DateTime.Now - lastDate;
                float passedSeconds = (float)passedTime.TotalSeconds;
                if (passedSeconds < _blockedTime)
                {
                    _timerRotine = StartCoroutine(UpdateRemainingTime(passedSeconds));
                    RemainingTimeText.gameObject.SetActive(true);
                }
                else
                {
                    DBVariablesHolder.AdBlocked.Value = 0;
                }
            }
        }

        private void OnDisable()
        {
            if (_timerRotine != null)
            {
                StopCoroutine(_timerRotine);
            }
        }

        private IEnumerator UpdateRemainingTime(float seconds)
        {
            WaitForSeconds duration = new WaitForSeconds(2);
            while (seconds > 1)
            {
                RemainingTimeText.text = string.Format("{0:00}:{1:00}", Mathf.Floor((seconds % 3600) / 60), Mathf.Floor(seconds % 60));           // seconds
                RemainingTimeText.text = string.Format("{0:00}:{1:00}", Mathf.Floor((seconds % 3600) / 60),Mathf.Floor(seconds % 60));
                seconds--;
                yield return duration;
            }

            if (seconds < 1)
            {
                DBVariablesHolder.AdBlocked.Value = 0;
                RemainingTimeText.gameObject.SetActive(false);
            }
        }
    }
}