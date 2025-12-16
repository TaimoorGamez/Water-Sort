using TMPro;
using Core.Events;
using UnityEngine;
using Core.Economy;
using Core.Plugins.Ads;
using Core.DB.Variables;

namespace Core.Screen
{
    public class PowerBtnHandler : MonoBehaviour
    {
        [SerializeField] GameObject CounterObj, PriceObj, AdObj;
        [SerializeField] TextMeshProUGUI RemaingText;
        [SerializeField] int Price;
        [SerializeField] string PowerName;

        private void OnEnable()
        {
            EventDictionariesHolder.RewardPowerEvent[PowerName] += RewardPower;
            EventDictionariesHolder.UpdatePowerStatusEvent[PowerName] += ChangeStatus;
            PowerStatus();
        }

        private void OnDisable()
        {
            EventDictionariesHolder.RewardPowerEvent[PowerName] -= RewardPower;
            EventDictionariesHolder.UpdatePowerStatusEvent[PowerName] -= ChangeStatus;
        }

        void PowerStatus()
        {
            CounterObj.SetActive(false);
            PriceObj.SetActive(false);
            AdObj.SetActive(false);
            if (DBVariableDictionariesHolder.PowerStatusData[PowerName].Value > 0)
            {
                RemaingText.text = DBVariableDictionariesHolder.PowerStatusData[PowerName].Value.ToString();
                CounterObj.SetActive(true);
            }
            else if (CurrenciesHolder.CashCurrency.Amount >= Price)
            {
                PriceObj.SetActive(true);
            }
            else
            {
                AdObj.SetActive(true);
            }
        }

        void ChangeStatus()
        {
            if (DBVariableDictionariesHolder.PowerStatusData[PowerName].Value > 0)
            {
                DBVariableDictionariesHolder.PowerStatusData[PowerName].Value--;
            }
            else if (CurrenciesHolder.CashCurrency.Amount >= Price)
            {
                CurrenciesHolder.CashCurrency.Amount-= Price;
            }
            PowerStatus();
        }

        public void OnClickPowerBtn()
        {
            if (DBVariableDictionariesHolder.PowerStatusData[PowerName].Value > 0 || CurrenciesHolder.CashCurrency.Amount >= Price)
            {
                EventDictionariesHolder.PowerEvents[PowerName]?.Invoke();
            }
            else
            {
                AdsManager.I?.ShowRewardedAd(PowerName);
            }
        }

        void RewardPower()
        {
            DBVariableDictionariesHolder.PowerStatusData[PowerName].Value++;
            PowerStatus();
        }
    }
}
