using UnityEngine;
using System.Collections.Generic;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.Variables
{
    [CreateAssetMenu(fileName = "SOAsyncGameObjectList", menuName = "ScriptableObjects/Variables/AsyncGameObjectList")]

    public class SOAsyncGameObjectIList : SOAsyncIList<GameObject>
    {
    }
}
