using UnityEngine;
using UnityEngine.UI;

namespace Core.Screen
{
    public class PaintingView : MonoBehaviour
    {
        [SerializeField] RawImage PaintingImg;
        [SerializeField] Image[] Stars;

        public void InitPainting(Texture paintingTex, int paintingStars)
        {
            PaintingImg.texture = paintingTex;
            for(int s = 0; s < paintingStars; s++)
            {
                Stars[s].enabled = true;
            }
            PaintingImg.enabled = true;
        }
    }
}
