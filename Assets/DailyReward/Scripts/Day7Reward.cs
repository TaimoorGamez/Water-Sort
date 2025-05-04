using TMPro;
using System;
using UnityEngine;
using Core.Economy;

namespace ProjectCore.DailyReward
{
    public class Day7Reward : DailyRewardHandler
    {
        [SerializeField] Currency GemEconomy;
        [SerializeField] int GemAmount;
        [SerializeField] TextMeshProUGUI[] GemText;

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void UpdateState()
        {
            base.UpdateState();
        }

        protected override void UpdateUI()
        {
            base.UpdateUI();

            foreach (TextMeshProUGUI txt in AmountText)
            {
                txt.text = Amount + " COINS";
            }

            foreach (TextMeshProUGUI txt in GemText)
            {
                txt.text = GemAmount.ToString() + " GEMS";
            }
        }

        protected override void OnClickBuyButton()
        {
            base.OnClickBuyButton();
            GemEconomy.Amount += GemAmount;
            try
            {
                FBEvents.EarnCoinsEvent("Gems", Amount, "DailyReward");
            }
            catch (Exception e)
            {
                Debug.Log("***Exception " + e);
            }

        }

        protected override void GrantDoubleReward()
        {
            GemEconomy.Amount += (GemAmount * 2);
            base.GrantDoubleReward();
        }
    }
}
