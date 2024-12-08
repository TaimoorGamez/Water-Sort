using UnityEngine;

namespace Core.GamePlay.WaterSort
{
    public class CapHandler : MonoBehaviour
    {
        [SerializeField] CapAnimation MyAnimation;

        public void PlayCelebration(Color currentColor)
        {
            Debug.Log("Here");
            MyAnimation.gameObject.SetActive(true);
            MyAnimation.PlayCapAnimation(currentColor);
        }

        public void HideCap()
        {
           MyAnimation.gameObject.SetActive(false);
        }
    }
}