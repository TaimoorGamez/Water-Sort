using TMPro;
using Core.Events;
using UnityEngine;
using Core.Plugins;
using Core.Economy;
using Core.DB.Variables;

namespace Core.Screen
{
    public class PowerBtnHandler : MonoBehaviour
    {
        [SerializeField] AdHandler RewardedAd;
        [SerializeField] DBInt RemaingPower;
        [SerializeField] Currency CashCurrency;
        [SerializeField] GameObject CounterObj, PriceObj, AdObj;
        [SerializeField] TextMeshProUGUI RemaingText;
        [SerializeField] int Price;
        [SerializeField] string PowerName;

        private void OnEnable()
        {
            if (EventDictionariesHolder.RewardPowerEvent.TryGetValue(PowerName, out var rEvt))
            {
                rEvt += RewardPower;
            }
            else
            {
                Debug.LogWarning($"Buy event key '{PowerName}' does not exist.");
            }
            if (EventDictionariesHolder.ChangePowerStatusEvent.TryGetValue(PowerName, out var evt))
            {
                evt += ChangeStatus;
            }
            else
            {
                Debug.LogWarning($"Buy event key '{PowerName}' does not exist.");
            }
            PowerStatus();
        }

        private void OnDisable()
        {
            if (EventDictionariesHolder.RewardPowerEvent.TryGetValue(PowerName, out var rEvt))
            {
                rEvt -= RewardPower;
            }
            else
            {
                Debug.LogWarning($"Buy event key '{PowerName}' does not exist.");
            }
            if (EventDictionariesHolder.ChangePowerStatusEvent.TryGetValue(PowerName, out var evt))
            {
                evt -= ChangeStatus;
            }
            else
            {
                Debug.LogWarning($"Buy event key '{PowerName}' does not exist.");
            }
        }

        void PowerStatus()
        {
            CounterObj.SetActive(false);
            PriceObj.SetActive(false);
            AdObj.SetActive(false);
            if (RemaingPower.Value > 0)
            {
                RemaingText.text = RemaingPower.Value.ToString();
                CounterObj.SetActive(true);
            }
            else if (CashCurrency.Amount >= Price)
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
            if (RemaingPower.Value > 0)
            {
                RemaingPower.Value--;
            }
            else if (CashCurrency.Amount >= Price)
            {
                CashCurrency.Amount-= Price;
            }
            PowerStatus();
        }

        public void OnClickPowerBtn()
        {
            if (RemaingPower.Value > 0 || CashCurrency.Amount >= Price)
            {
                if (EventDictionariesHolder.PowerEvents.TryGetValue(PowerName, out var evt))
                {
                    evt?.Invoke();
                }
                else
                {
                    Debug.LogWarning($"Buy event key '{PowerName}' does not exist.");
                }
            }
            else
            {
                RewardedAd.ShowAd(PowerName);
            }
        }

        void RewardPower()
        {
            RemaingPower.Value++;
            PowerStatus();
        }
    }
}
