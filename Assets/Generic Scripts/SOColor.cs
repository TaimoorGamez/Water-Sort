using UnityEngine;

namespace Core.Variables
{
    [CreateAssetMenu(fileName = "SOColor", menuName = "ScriptableObjects/Variables/Colors")]
    public class SOColor : ScriptableObject
    {
        [SerializeField] Color32 CurrentValue, DefaultValue;
        [SerializeField] bool ResetDefault = true;

        public Color32 Value
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
