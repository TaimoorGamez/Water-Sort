using UnityEngine;
using Core.Events;
using Core.DB.Variables;

namespace Core.Economy
{
    [CreateAssetMenu(fileName = "NewEconomy", menuName = "ScriptableObjects/Economy")]
    public class Currency : ScriptableObject
    {
        [SerializeField] DBInt _Amount;
        [SerializeField] SOIntegerEvents DepositeEvent, TransactionEvent;
        [SerializeField] SO2IntergerEvent TaskEvent;

        public int Amount
        {
            get
            {
                return _Amount.Value;
            }
            set
            {
                if (value > _Amount.Value)
                {
                    DepositeEvent.InvokeSOEvent(value);
                }
                else if (value < _Amount.Value)
                {
                    TransactionEvent.InvokeSOEvent(value);
                    TaskEvent.InvokeSOEvent(2, value);
                }
                _Amount.Value = (value);
            }
        }
    }
}
