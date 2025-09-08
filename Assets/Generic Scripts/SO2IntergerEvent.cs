using UnityEngine;

namespace Core.Events
{
    [CreateAssetMenu(fileName = "2IntegerEvent", menuName = "ScriptableObjects/Events/2IntegerEvents")]
    public class SO2IntergerEvent : ScriptableObject
    {
        public GameEventWith2Ints EventHandler;

        public void InvokeSOEvent(int index, int val)
        {
            EventHandler?.Invoke(index, val);
        }
    }
}