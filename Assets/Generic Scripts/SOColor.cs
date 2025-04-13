using UnityEngine;

namespace Core.GamePlay.Coloring
{
    [CreateAssetMenu(fileName = "SOColor", menuName = "ScriptableObjects/GamePlay/SingleColor")]
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
