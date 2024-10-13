using UnityEngine;
using UnityEngine.UI;

namespace Core.GamePlay.Coloring
{
    public class ColorFilling : MonoBehaviour
    {
        [SerializeField] RawImage RawImage; // The UI component displaying this part's texture

        Texture2D _partTexture; // The texture of this specific part
       [SerializeField] int _partWidth, _partHeight, _brushSize;

        private void Start()
        {
            // Initialize partWidth and partHeight based on the RawImage size
            _partWidth = (int)RawImage.rectTransform.rect.width;
            _partHeight = (int)RawImage.rectTransform.rect.height;

            // Create a new texture with the same size as the part
            _partTexture = new Texture2D(RawImage.texture.width, RawImage.texture.height, TextureFormat.RGBA32, false);

            // Copy pixels from the existing RawImage texture to partTexture
            Texture2D originalTexture = (Texture2D)RawImage.texture;
            Color32[] originalPixels = originalTexture.GetPixels32();
            byte alphaThreshold = 50;
            // Iterate through all the pixels
            for (int i = 0; i < originalPixels.Length; i++)
            {
                // If alpha > 0, make the pixel white, preserving alpha
                if (originalPixels[i].a > alphaThreshold)
                {
                    originalPixels[i] = Color.white; // White color, preserving alpha
                }
            }

            // Set the modified pixels to the new texture
            _partTexture.SetPixels32(originalPixels);
            _partTexture.Apply(); // Apply changes to the texture

            // Assign the new texture to the RawImage component
            RawImage.texture = _partTexture;
        }

        private void Update()
        {
            //if (Input.get)
            //{

            //}
        }

        public void ApplyBrush(Vector2 brushPosition, Color brushColor)
        {
            // Convert brush position to local coordinates relative to the part's RectTransform
            Vector2 localPosition = RawImage.rectTransform.InverseTransformPoint(brushPosition);

            // Get pixel coordinates on the texture
            int x = Mathf.Clamp((int)localPosition.x, 0, _partWidth - 1);
            int y = Mathf.Clamp((int)localPosition.y, 0, _partHeight - 1);

            // Apply the brush color to the pixels around the brush position
            for (int i = -_brushSize; i <= _brushSize; i++)
            {
                for (int j = -_brushSize; j <= _brushSize; j++)
                {
                    if (x + i >= 0 && x + i < _partWidth && y + j >= 0 && y + j < _partHeight)
                    {
                        // Optional: you could also lerp between the current pixel color and the new color
                        Color existingColor = _partTexture.GetPixel(x + i, y + j);
                        _partTexture.SetPixel(x + i, y + j, Color.Lerp(existingColor, brushColor, brushColor.a));
                    }
                }
            }

            // Apply the texture changes after modifying pixels
            _partTexture.Apply();
        }
    }
}
