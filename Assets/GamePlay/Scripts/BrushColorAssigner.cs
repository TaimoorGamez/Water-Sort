using Core.Events;
using UnityEngine;
using UnityEngine.UI;

namespace Core.GamePlay.Coloring
{
    public class BrushColorAssigner : MonoBehaviour
    {
        [SerializeField] SOColor CurrentColor;
        [SerializeField] Image BurshImage;


        private void OnEnable()
        {
            SimpleEventsHolder.ColorSelectedEvent += ColorSelected;
        }

        private void OnDisable()
        {
            SimpleEventsHolder.ColorSelectedEvent -= ColorSelected;
        }

        void ColorSelected()
        {
            BurshImage.color = CurrentColor.Value;
        }
    }
}
