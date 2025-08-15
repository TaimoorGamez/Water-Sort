using TMPro;
using UnityEngine;
using Core.CustomUI;
using UnityEngine.UI;

namespace Core.SpinWheel
{
    public class SpinWheelSegment : MonoBehaviour
    {
        [SerializeField] UIGradient BackgroundGradient;
        [SerializeField] Image IconImage;
        [SerializeField] TextMeshProUGUI AmountText;

        public void Initialize(Sprite icon, int amount, Gradient gradient)
        {
            IconImage.sprite = icon;
            AmountText.text = amount.ToString();
            BackgroundGradient.Gradient = gradient;
        }
    }
}
