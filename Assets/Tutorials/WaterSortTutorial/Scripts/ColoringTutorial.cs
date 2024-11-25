using UnityEngine;
using Core.Events;

namespace Core.GamePlay.Coloring
{
    public class ColoringTutorial : MonoBehaviour
    {
        [SerializeField] SOEvents StartColoringEvent, ColorSelectedEvent;
        [SerializeField] GameObject TutorialHand;


        private void OnEnable()
        {
            StartColoringEvent.EventHandler += StartColoring;
            ColorSelectedEvent.EventHandler += ColorSelected;
        }

        private void OnDisable()
        {
            StartColoringEvent.EventHandler += StartColoring;
            ColorSelectedEvent.EventHandler += ColorSelected;
        }

        void StartColoring()
        {
            Invoke(nameof(OnTutorialHand),1);
        }

        void OnTutorialHand()
        {
            TutorialHand.SetActive(true);
        }

        void ColorSelected()
        {
            TutorialHand.SetActive(false);
        }
    }
}
