using Core.Events;
using UnityEngine;
using UnityEngine.UI;
using Core.Variables;

namespace Core.GamePlay.Coloring
{
    public class BrushColorAssigner : MonoBehaviour
    {
        [SerializeField] SOEvents ColorSelectedEvent;
        [SerializeField] SOColor CurrentColor;
        [SerializeField] Image BurshImage;


        private void OnEnable()
        {
            ColorSelectedEvent.EventHandler += ColorSelected;
        }

        private void OnDisable()
        {
            ColorSelectedEvent.EventHandler -= ColorSelected;
        }

        void ColorSelected()
        {
            BurshImage.color = CurrentColor.Value;
        }
    }
}
