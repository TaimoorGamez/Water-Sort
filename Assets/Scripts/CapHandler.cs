using UnityEngine;
namespace Core.GamePlay.WaterSort
{
    public class CapHandler : MonoBehaviour
    {
        [SerializeField] CapAnimation MyAnimation;

        public void PlayCelebration(Color currentColor)
        {
            MyAnimation.gameObject.SetActive(true);
            MyAnimation.SetColor(currentColor);
        }

        public void HideCap()
        {
            MyAnimation.gameObject.SetActive(false);
        }
    }
}