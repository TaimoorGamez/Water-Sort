using UnityEngine;

namespace Core.Variables
{
    [CreateAssetMenu(fileName = "SOVector", menuName = "ScriptableObjects/Variables/Vectors")]
    public class SOVector : ScriptableObject
    {
        [SerializeField] Vector3 CurrentValue, DefaultValue;
        [SerializeField] bool ResetDefault = true;

        public Vector3 Value
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
