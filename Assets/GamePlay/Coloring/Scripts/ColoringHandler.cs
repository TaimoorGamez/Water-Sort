using UnityEngine;
using Core.Variables;
using System.Collections;

namespace Core.GamePlay.Coloring
{
    public class ColoringHandler : MonoBehaviour
    {
        [SerializeField] SOColor CurrentColor;
        [SerializeField] ColorFilling ColoringPart;
        [SerializeField] RectTransform BrushTransform;
        [SerializeField] Vector2Int VerticalRange, HorizontalRange; // Use these for boundary clamping

        float _speed = 500, _brushSize = 25;
        bool _isDragging = false;
        Coroutine _movingRoutine;
        RectTransform _coloringParTransform;
        Camera _currentCamera;
        Texture2D _partTexture;
        byte _alphaThreshold = 50;

        private void Start()
        {
            _currentCamera = Camera.main;
            _coloringParTransform = ColoringPart.transform as RectTransform;
            _partTexture = ColoringPart.GetCurrenTexture();
        }

        public void OnBeginDrag()
        {
            // Start the coloring coroutine
            _isDragging = true;
            _movingRoutine = StartCoroutine(MovingRoutine());
        }

        public void OnEndDrag()
        {
            _isDragging = false;
            if (_movingRoutine != null)
            {
                StopCoroutine(_movingRoutine);
            }
        }

        IEnumerator MovingRoutine()
        {
            float waiting = 0.01f; 
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_coloringParTransform, Input.mousePosition, _currentCamera, out Vector2 initialPoint);
            Vector2 initialOffset = BrushTransform.anchoredPosition - initialPoint;
            while (_isDragging)
            {
                // Convert screen position to local position relative to the RectTransform
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_coloringParTransform, Input.mousePosition, _currentCamera, out Vector2 localPoint);

                // Calculate target position with offset (if needed)
                Vector2 targetPosition = localPoint + initialOffset;

                // Clamp the position within defined boundaries
                float clampedX = Mathf.Clamp(targetPosition.x, HorizontalRange.x, HorizontalRange.y);
                float clampedY = Mathf.Clamp(targetPosition.y, VerticalRange.x, VerticalRange.y);

                // Update BrushTransform position
                BrushTransform.anchoredPosition = new Vector2(clampedX, clampedY);

                // Convert BrushTransform position to UV coordinates (0-1 range in texture space)
                Vector2 brushUV = new Vector2(
                    (BrushTransform.anchoredPosition.x - _coloringParTransform.rect.x) / _coloringParTransform.rect.width,
                    (BrushTransform.anchoredPosition.y - _coloringParTransform.rect.y) / _coloringParTransform.rect.height
                );

                // Convert UV coordinates to texture pixel coordinates
                int texX = Mathf.RoundToInt(brushUV.x * _partTexture.width);
                int texY = Mathf.RoundToInt(brushUV.y * _partTexture.height);

                // Apply color at the corrected texture coordinates
                ApplyBrush(texX, texY);

                yield return new WaitForSeconds(waiting);
            }
        }


        void ApplyBrush(int centerX, int centerY)
        {
            int brushRadius = Mathf.RoundToInt(_brushSize);

            for (int y = -brushRadius; y <= brushRadius; y++)
            {
                for (int x = -brushRadius; x <= brushRadius; x++)
                {
                    int pixelX = centerX + x;
                    int pixelY = centerY + y;

                    // Check if the pixel is inside the circular brush area
                    if (pixelX >= 0 && pixelX < _partTexture.width && pixelY >= 0 && pixelY < _partTexture.height)
                    {
                        float dist = Mathf.Sqrt(x * x + y * y);
                        if (dist <= brushRadius && _partTexture.GetPixel(pixelX, pixelY).a > _alphaThreshold)
                        {
                            _partTexture.SetPixel(pixelX, pixelY, CurrentColor.Value);
                        }
                    }
                }
            }

            _partTexture.Apply();
        }
    }
}
