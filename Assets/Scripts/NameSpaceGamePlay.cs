using UnityEngine;

namespace Core.GamePlay
{
    public class ColorAssigner : MonoBehaviour
    {
        [SerializeField] protected Renderer MySkin;
        [SerializeField] protected Color SkinColor;

        protected MaterialPropertyBlock _propBlock;

       protected virtual void Start()
        {
            _propBlock = new MaterialPropertyBlock();
        }
    }
}
