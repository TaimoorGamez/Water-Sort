using UnityEngine;
using Core.DB.Variables;

namespace Core.GamePlay.WaterSort
{
    public class CapHandler : MonoBehaviour
    {
        [SerializeField] DBInt CurrentCap;
        [SerializeField] string CaPath;

        CapAnimation _myAnimation;

        private void Start()
        {
            _myAnimation = Instantiate(Resources.Load<CapAnimation>(CaPath + CurrentCap.Value), transform);
        }

        public void PlayCelebration(Color currentColor)
        {
            _myAnimation.gameObject.SetActive(true);
            _myAnimation.PlayCapAnimation(currentColor);
        }

        public void HideCap()
        {
           _myAnimation.gameObject.SetActive(false);
        }
    }
}