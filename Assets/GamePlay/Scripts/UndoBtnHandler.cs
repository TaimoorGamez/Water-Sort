using TMPro;
using UnityEngine;
using Core.Events;
using Core.Economy;
using Core.DB.Variables;

namespace Core.Screen
{
    public class UndoBtnHandler : MonoBehaviour
    {
        [SerializeField] DBInt RemaingUndo;
        [SerializeField] SOEvents UndoEvent, UndoStatusEvent;
        [SerializeField] SOIntegerEvents ToastMsgEvent;
        [SerializeField] Currency CashEconomy;
        [SerializeField] GameObject CounterObj, PriceObj, AdObj;
        [SerializeField] TextMeshProUGUI RemaingText;

        int _price = 100;

        private void OnEnable()
        {
            UndoStatusEvent.EventHandler += ChangeStatus;
            PowerStatus();
        }

        private void OnDisable()
        {
            UndoStatusEvent.EventHandler -= ChangeStatus;
        }

        void PowerStatus()
        {
            CounterObj.SetActive(false);
            PriceObj.SetActive(false);
            AdObj.SetActive(false);
            if (RemaingUndo.Value > 0)
            {
                RemaingText.text = RemaingUndo.Value.ToString();
                CounterObj.SetActive(true);
            }
            else if (CashEconomy.Amount >= _price)
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
            if (RemaingUndo.Value > 0)
            {
                RemaingUndo.Value--;
            }
            else if (CashEconomy.Amount >= _price)
            {
                CashEconomy.Amount-= _price;
            }
            PowerStatus();
        }

        public void OnClickUndoBtn()
        {
            if (RemaingUndo.Value > 0 || CashEconomy.Amount >= _price)
            {
                UndoEvent.InvokeSOEvent();
            }
            else
            {
                ToastMsgEvent.InvokeSOEvent(3);
            }
        }
    }
}
