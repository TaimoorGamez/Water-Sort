using UnityEngine;

namespace Core.DB.Variables
{
    [CreateAssetMenu(fileName = "DBString", menuName = "ScriptableObjects/DB/Variables/String")]
    public class DBString : ScriptableObject
    {
        [SerializeField] string CurrentValue, DefaultValue, PrefName;
        [SerializeField] bool ResetDefault = true;

        public string Value
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
                Value = PlayerPrefs.GetString(PrefName, DefaultValue);
            }
        }

        void SaveValue(string value)
        {
            PlayerPrefs.SetString(PrefName, value);
            PlayerPrefs.Save();
        }
    }
}
