using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Core.GamePlay.Coloring
{
    public class ColorFilling : MonoBehaviour
    {
        [SerializeField] RawImage ColoringPart;

        Texture2D _partTexture;
        List<int> _coloredPixels = new List<int>();

        private void Start()
        {
            _partTexture = new Texture2D(ColoringPart.texture.width, ColoringPart.texture.height, TextureFormat.RGBA32, false);

            Texture2D originalTexture = (Texture2D)ColoringPart.texture;
            Color32[] originalPixels = originalTexture.GetPixels32();
            byte alphaThreshold = 50;

            for (int i = 0; i < originalPixels.Length; i++)
            {
                // If alpha > 0, make the pixel white, preserving alpha
                if (originalPixels[i].a > alphaThreshold)
                {
                    originalPixels[i] = Color.white;
                    _coloredPixels.Add(i);
                }
            }

            _partTexture.SetPixels32(originalPixels);
            _partTexture.Apply();

            ColoringPart.texture = _partTexture;
        }

        public Texture2D GetCurrenTexture()
        {
            return _partTexture;
        }

        public List<int> GetColoredPixles()
        {
            return _coloredPixels;
        }
    }
}
