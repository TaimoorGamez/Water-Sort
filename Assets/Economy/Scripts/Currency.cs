using UnityEngine;
using Core.Events;
using Core.DB.Variables;

namespace Core.Economy
{
    [CreateAssetMenu(fileName = "NewEconomy", menuName = "ScriptableObjects/Economy")]
    public class Currency : ScriptableObject
    {
        [SerializeField] DBInt _Amount;

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
                    SingleIntegerEventsHolder.DepositEvent?.Invoke(value);
                }
                else if (value < _Amount.Value)
                {
                    SingleIntegerEventsHolder.TransactionEvent?.Invoke(value);
                    DoubleIntegerEventHolder.TaskEvent?.Invoke(2, value);
                }
                _Amount.Value = (value);
            }
        }
    }
}
