using UnityEngine;

namespace Core.Events
{
    [CreateAssetMenu(fileName = "IntegerEvent", menuName = "ScriptableObjects/Events/IntegerEvents")]
    public class SOIntegerEvents : ScriptableObject
    {
        public GameEventInteger EventHandler;

        public void InvokeSOEvent(int val)
        {
            EventHandler?.Invoke(val);
        }
    }
}
