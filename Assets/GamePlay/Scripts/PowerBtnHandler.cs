using TMPro;
using UnityEngine;
using Core.Events;
using Core.Economy;
using Core.DB.Variables;

namespace Core.Screen
{
    public class PowerBtnHandler : MonoBehaviour
    {
        [SerializeField] DBInt RemaingPower;
        [SerializeField] SOEvents PowerEvent, ChangePowerStatusEvent;
        [SerializeField] Currency CashCurrency;
        [SerializeField] GameObject CounterObj, PriceObj, AdObj;
        [SerializeField] TextMeshProUGUI RemaingText;
        [SerializeField] int Price;

        private void OnEnable()
        {
            ChangePowerStatusEvent.EventHandler += ChangeStatus;
            PowerStatus();
        }

        private void OnDisable()
        {
            ChangePowerStatusEvent.EventHandler -= ChangeStatus;
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
                PowerEvent.InvokeSOEvent();
            }
        }
    }
}
