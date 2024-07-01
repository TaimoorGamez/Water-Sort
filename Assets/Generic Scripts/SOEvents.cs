using UnityEngine;

namespace Core.Events
{
    [CreateAssetMenu(fileName = "NormalEvent", menuName = "ScriptableObjects/Events/NormalEvents")]
    public class SOEvents : ScriptableObject
    {
        public GameEvent EventHandler;

        public void InvokeEvent()
        {
            EventHandler?.Invoke();
        }
    }
}
