using UnityEngine;

namespace Core.Variables
{
    [CreateAssetMenu(fileName = "SOInt", menuName = "ScriptableObjects/Variables/Ints")]
    public class SOInterger : ScriptableObject
    {
        [SerializeField] int CurrentValue, DefaultValue;
        [SerializeField] bool ResetDefault = true;

        public int Value 
        { 
            get => CurrentValue;
            set 
            {
                CurrentValue = value;
            }
        }


        private void OnEnable()
        {
            if (ResetDefault)
            {
                Value = DefaultValue;
            }
        }
    }
}
