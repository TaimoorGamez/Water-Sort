using UnityEngine;

namespace Core.Screen
{
    public class UiScreens : MonoBehaviour
    {
        public RectTransform Body;
        protected float _transitionDuration = 1f;

        public virtual void OnOpen()
        {

        }

        public virtual void OnClose()
        {
            
        }
    }
}