using UnityEngine;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.Variables
{
    [CreateAssetMenu(fileName = "SOAsyncIList", menuName = "ScriptableObjects/Variables/AsyncIList")]

    public class SOAsyncIList : MonoBehaviour
    {
        [SerializeField] AsyncOperationHandle<IList<GameObject>> CurrentList, DefaultList;
        [SerializeField] bool ResetDefault = true;

        public AsyncOperationHandle<IList<GameObject>> ValueList
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
                ValueList = DefaultList;
            }
        }
    }
}
