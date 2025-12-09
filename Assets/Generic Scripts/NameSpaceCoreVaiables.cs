using UnityEngine;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.Variables
{
    public class SODictionaryBase<TKey, TValue> : ScriptableObject
    {
        [SerializeField] protected Dictionary<TKey, TValue> CurrentValue, DefaultValue;
        [SerializeField] protected bool ResetDefault = true;

        public Dictionary<TKey, TValue> DictionaryValue
        {
            get => CurrentValue;
            set
            {
                CurrentValue = value;
            }
        }


        protected void OnEnable()
        {
            if (ResetDefault)
            {
                DictionaryValue = DefaultValue;
            }
        }
    }

    public class SOAsyncIList<T> : ScriptableObject
    {
        [SerializeField] protected AsyncOperationHandle<IList<T>> CurrentList, DefaultList;
        [SerializeField] protected bool ResetDefault = true;

        public AsyncOperationHandle<IList<T>> ListValue
        {
            get => CurrentList;
            set
            {
                CurrentList = value;
            }
        }


        private void OnEnable()
        {
            if (ResetDefault)
            {
                ListValue = DefaultList;
            }
        }
    }
}
