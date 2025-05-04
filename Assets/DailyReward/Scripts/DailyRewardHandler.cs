using TMPro;
using System;
using UnityEngine;
using Core.Events;
using Core.Economy;
using UnityEngine.UI;
using Core.Variables;
using Core.DB.Variables;
using Core.Plugins.Firebase;

namespace ProjectCore.DailyReward
{
    public class DailyRewardHandler : MonoBehaviour
    {
        [SerializeField] protected Currency CoinsEconomy;
        [SerializeField] protected SOInterger GiveReward;
        [SerializeField] protected FireBaseEvents FBEvents;
        [SerializeField] protected SOEvents UpDateState, DoubleRewardEvent;
        [SerializeField] protected DBInt ToDay , RewardClaimed;
        [SerializeField] protected TextMeshProUGUI[] DayText, AmountText;
        [SerializeField] protected Button BuyButton, DoubleRewardButton;
        [SerializeField] protected GameObject[] RewardState;
        [SerializeField] protected int RewardDay, Amount;

        protected bool _doubleRewardClicked = false;

        protected virtual void OnEnable()
        {
            UpdateUI();
            UpdateState();
        }

        protected virtual void UpdateUI()
        {
            foreach (TextMeshProUGUI txt in DayText)
            {
                txt.text = "DAY " + RewardDay.ToString();
            }

            foreach (TextMeshProUGUI txt in AmountText)
            {
                txt.text = Amount.ToString();
            }
        }

        protected void Update()
        {
            if (GiveReward.Value == 1 && ToDay.Value == RewardDay && RewardClaimed.Value == 0 && !_doubleRewardClicked)
            {
                GiveReward.Value = 0;
                GrantDoubleReward();
            }
        }

        protected virtual void UpdateState()
        {
            if (ToDay.Value == RewardDay && RewardClaimed.Value == 0)
            {
                ActiveState(1);
                BuyButton.onClick.AddListener(OnClickBuyButton);
                DoubleRewardEvent.EventHandler += GrantDoubleReward;
            }
            else
            {
                if (ToDay.Value < RewardDay)
                {
                    ActiveState(0);
                }
                else
                {
                    ActiveState(2);
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
            if (RewardClaimed.Value == 0)
            {
                RewardClaimed.Value = (1);
                CoinsEconomy.Amount += Amount;
                UpdateState();
                BuyButton.onClick.RemoveListener(OnClickBuyButton);
                DoubleRewardEvent.EventHandler -= GrantDoubleReward;
                UpDateState.InvokeSOEvent();
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
            if (ToDay.Value == RewardDay && RewardClaimed.Value == 0 && !_doubleRewardClicked)
            {
                _doubleRewardClicked = true;
                RewardClaimed.Value = (1);
                CoinsEconomy.Amount += (Amount * 2);
                UpdateState();
                //Debug.Log("Amount " + (Amount * 2));
                UpDateState.InvokeSOEvent();
                BuyButton.onClick.RemoveListener(OnClickBuyButton);
                DoubleRewardEvent.EventHandler -= GrantDoubleReward;
            }
        }

    }
}
