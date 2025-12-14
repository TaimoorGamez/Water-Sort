using TMPro;
using System;
using UnityEngine;
using Core.DB.Variables;

namespace Core.DailyReward
{
    public class Day7Reward : DailyRewardHandler
    {
        [SerializeField] DBInt RemainingTubes;
        [SerializeField] int TubeCounts;
        [SerializeField] TextMeshProUGUI[] TubesText;

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
            for (int i = 0; i < AmountText.Length; i++)
            {
                AmountText[i].text = Amount.ToString();
            }

            for (int i = 0; i < TubesText.Length; i++)
            {
                TubesText[i].text = TubeCounts.ToString();
            }
        }

        protected override void OnClickBuyButton()
        {
            base.OnClickBuyButton();
            RemainingTubes.Value += TubeCounts;
            try
            {
                FBEvents.EarnCoinsEvent("Tube", Amount, "DailyReward");
            }
            catch (Exception e)
            {
                Debug.Log("***Exception " + e);
            }
        }

        protected override void GrantDoubleReward()
        {
            RemainingTubes.Value += (TubeCounts * 2);
            base.GrantDoubleReward();
        }
    }
}
