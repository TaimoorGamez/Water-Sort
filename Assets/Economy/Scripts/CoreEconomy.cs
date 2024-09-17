using UnityEngine;
using Core.DB.Variables;

namespace Core.Economy
{
    public class CoreEconomy : ScriptableObject
    {
        [SerializeField] DBInt _Amount;

        public int Amount
        {
            get
            {
                if (_Amount.Value > -1)
                {
                    return _Amount.Value;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                if (value > -1)
                {
                    _Amount.Value = value;
                }
            }
        }
    }
}
