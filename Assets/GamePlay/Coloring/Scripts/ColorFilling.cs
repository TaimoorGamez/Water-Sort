using UnityEngine;
using UnityEngine.UI;
using Core.Variables;
using System.Collections;

namespace Core.GamePlay.Coloring
{
    public class ColorFilling : MonoBehaviour
    {
        [SerializeField] SOColor CurrentColor;
        [SerializeField] SOVector BurshPosition;
        [SerializeField] SOInterger IsDragging;
        [SerializeField] RawImage RawImage;

        Texture2D _partTexture;
        int _partWidth, _partHeight, _brushSize = 25;
        Coroutine _coloringRotine;

        private void Start()
        {
            _partWidth = (int)RawImage.rectTransform.rect.width;
            _partHeight = (int)RawImage.rectTransform.rect.height;

            _partTexture = new Texture2D(RawImage.texture.width, RawImage.texture.height, TextureFormat.RGBA32, false);

            Texture2D originalTexture = (Texture2D)RawImage.texture;
            Color32[] originalPixels = originalTexture.GetPixels32();
            byte alphaThreshold = 50;

            for (int i = 0; i < originalPixels.Length; i++)
            {
                // If alpha > 0, make the pixel white, preserving alpha
                if (originalPixels[i].a > alphaThreshold)
                {
                    originalPixels[i] = Color.white;
                }
            }

            _partTexture.SetPixels32(originalPixels);
            _partTexture.Apply();

            RawImage.texture = _partTexture;
            StartColoring();
        }

        public void StartColoring()
        {
            _coloringRotine = StartCoroutine(ColoringCorotine());
        }

        IEnumerator ColoringCorotine()
        {
            float waiting = 0.01f;

            while (IsDragging.Value == 1)
            {

                yield return new WaitForSeconds(waiting);
            }
        }

        public void StopColoring()
        {
            if (_coloringRotine != null)
            {
                StopCoroutine(_coloringRotine);
            }
        }

        void ApplyBrush()
        {
            // Convert brush position to local coordinates relative to the part's RectTransform
            Vector2 localPosition = RawImage.rectTransform.InverseTransformPoint(BurshPosition.Value);

            // Get pixel coordinates on the texture
            int x = Mathf.Clamp((int)localPosition.x, 0, _partWidth - 1);
            int y = Mathf.Clamp((int)localPosition.y, 0, _partHeight - 1);

            // Check if the pixel at the brush position has an alpha greater than the threshold
            Color32 existingColor = _partTexture.GetPixel(x, y);
            byte alphaThreshold = 50; // Set your alpha threshold

            // Compare alpha directly as byte
            if (existingColor.a > alphaThreshold)
            {
                // Apply the brush color to the pixels around the brush position
                for (int i = -_brushSize; i <= _brushSize; i++)
                {
                    for (int j = -_brushSize; j <= _brushSize; j++)
                    {
                        int newX = x + i;
                        int newY = y + j;

                        // Ensure the new pixel coordinates are within the texture bounds
                        if (newX >= 0 && newX < _partWidth && newY >= 0 && newY < _partHeight)
                        {
                            // Get the current pixel color
                            Color32 currentPixelColor = _partTexture.GetPixel(newX, newY);

                            // Apply the brush color directly, considering transparency if needed
                            _partTexture.SetPixel(newX, newY, CurrentColor.Value);
                        }
                    }
                }

                // Apply the texture changes after modifying pixels
                _partTexture.Apply();
            }
        }
    }
}
