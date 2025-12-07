using UnityEngine;

namespace Core.DB.Variables
{
    [CreateAssetMenu(fileName = "DBInt", menuName = "ScriptableObjects/DB/Variables/Ints")]
    public class DBInt : ScriptableObject
    {
        [SerializeField] int CurrentValue, DefaultValue;
        [SerializeField] bool ResetDefault = true;
        [SerializeField] string PrefName;

        public int Value
        {
            get => CurrentValue;
            set
            {
                CurrentValue = value;
                SaveValue(value);
            }
        }


        private void OnEnable()
        {
            if (ResetDefault)
            {
                Value = PlayerPrefs.GetInt(PrefName,DefaultValue);
            }
        }

        void SaveValue(int value)
        {
            PlayerPrefs.SetInt(PrefName, value);
            PlayerPrefs.Save();
        }
    }
}
