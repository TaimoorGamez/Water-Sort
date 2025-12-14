using TMPro;
using System;
using UnityEngine;
using Core.Events;
using Core.Economy;
using UnityEngine.UI;
using Core.DB.Variables;
using Core.Plugins.Firebase;

namespace Core.DailyReward
{
    public class DailyRewardHandler : MonoBehaviour
    {
        [SerializeField] protected FireBaseEvents FBEvents;
        [SerializeField] protected TextMeshProUGUI[] DayText, AmountText;
        [SerializeField] protected Button ClaimButton;
        [SerializeField] protected GameObject[] RewardState;
        [SerializeField] protected int RewardDay, Amount;

        protected bool _doubleRewardClicked = false, _giveReward;

        protected virtual void OnEnable()
        {
            UpdateUI();
            UpdateState();
        }

        protected virtual void UpdateUI()
        {
            for (int i = 0; i < DayText.Length; i++)
            {
                DayText[i].text = "DAY " + RewardDay.ToString();
            }

            for (int i = 0; i < AmountText.Length; i++)
            {
                AmountText[i].text = Amount.ToString();
            }
        }

        protected void Update()
        {
            if (_giveReward && DBIntsHolder.ToDay.Value == RewardDay && DBIntsHolder.RewardClaimed.Value == 0 && !_doubleRewardClicked)
            {
                _giveReward = false;
                GrantDoubleReward();
            }
        }

        protected virtual void UpdateState()
        {
            if (DBIntsHolder.ToDay.Value == RewardDay && DBIntsHolder.RewardClaimed.Value == 0)
            {
                ActiveState(1);
                ClaimButton.onClick.AddListener(OnClickBuyButton);
                SimpleEventsHolder.DoubleDailyRewardEvent += GrantDoubleReward;
            }
            else
            {
                if (DBIntsHolder.ToDay.Value < RewardDay)
                {
                    ActiveState(2);
                }
                else
                {
                    ActiveState(0);
                }
            }
        }

        private void ActiveState(int stateIndex)
        {
            for (int state = 0; state < RewardState.Length; state++)
            {
                RewardState[state].SetActive(false);
            }
            RewardState[stateIndex].SetActive(true);
        }

        protected virtual void OnClickBuyButton()
        {
            if (DBIntsHolder.RewardClaimed.Value == 0)
            {
                DBIntsHolder.RewardClaimed.Value = 1;
                CurrenciesHolder.CashCurrency.Amount += Amount;
                UpdateState();
                ClaimButton.onClick.RemoveListener(OnClickBuyButton);
                SimpleEventsHolder.DoubleDailyRewardEvent -= GrantDoubleReward;
                SimpleEventsHolder.UpDateDailyRewardState?.Invoke();
                try
                {
                    FBEvents.EarnCoinsEvent("Coins", Amount, "DailyReward");
                }
                catch(Exception e)
                {
                    Debug.Log("***Exception " + e);
                }
            }
        }

        protected virtual void GrantDoubleReward()
        {
            if (DBIntsHolder.ToDay.Value == RewardDay && DBIntsHolder.RewardClaimed.Value == 0 && !_doubleRewardClicked)
            {
                _doubleRewardClicked = true;
                DBIntsHolder.RewardClaimed.Value = 1;
                CurrenciesHolder.CashCurrency.Amount += (Amount * 2);
                UpdateState();
                SimpleEventsHolder.UpDateDailyRewardState?.Invoke();
                ClaimButton.onClick.RemoveListener(OnClickBuyButton);
                SimpleEventsHolder.DoubleDailyRewardEvent -= GrantDoubleReward;
            }
        }

    }
}
