using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Core.Events;
using Core.Variables;
using System.Collections;

namespace Core.GamePlay.Coloring
{
    public class ColoringHandler : MonoBehaviour
    {
        [SerializeField] SOEvents StartColoringEvent, ColorSelectedEvent;
        [SerializeField] SOColor CurrentColor;
        [SerializeField] ColorFilling[] ColoringPart;
        [SerializeField] RectTransform BrushTransform, ColoringImage;
        [SerializeField] Vector2Int VerticalRange, HorizontalRange;
        [SerializeField] TextMeshProUGUI InfoText;
        [SerializeField] string[] InfoMsgs;
        [SerializeField] GameObject PaintBrush, RefferanceBar;
        [SerializeField] Image RefferanceImg;
        [SerializeField] Sprite[] RefferanceSprites;

        float _speed = 5, _brushSize = 15, _alphaThreshold = 0.1f, _preparationTime = 1, _finalPos = 100, _finalScale = 1.5f;
        bool _canColor = false;
        Coroutine _movingRoutine;
        RectTransform _coloringParTransform;
        Camera _currentCamera;
        Texture2D _partTexture;
        int _paintingCounter = 0;

        private void OnEnable()
        {
            StartColoringEvent.EventHandler += StartColoring;
            ColorSelectedEvent.EventHandler += ColorSelected;
        }

        private void OnDisable()
        {
            StartColoringEvent.EventHandler -= StartColoring;
            ColorSelectedEvent.EventHandler -= ColorSelected;
            _canColor = false;
            if (_movingRoutine != null)
            {
                StopCoroutine(_movingRoutine);
                _movingRoutine = null;
            }
        }

        private void Start()
        {
            _currentCamera = Camera.main;
            _coloringParTransform = ColoringPart[_paintingCounter].transform as RectTransform;
        }

        IEnumerator MovingRoutine()
        {
            float waiting = 0.01f;
            Vector2 initialOffset = Vector2.zero;

            while (_canColor)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(_coloringParTransform, Input.mousePosition, _currentCamera, out Vector2 initialPoint);
                    initialOffset = BrushTransform.anchoredPosition - initialPoint;
                    InfoText.gameObject.SetActive(false); 
                }

                if (Input.GetMouseButton(0))
                { // Convert screen position to local position relative to the RectTransform
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(_coloringParTransform, Input.mousePosition, _currentCamera, out Vector2 localPoint);
                    // Calculate target position with offset (if needed)
                    Vector2 targetPosition = localPoint + initialOffset;

                    // Clamp the position within defined boundaries
                    float clampedX = Mathf.Clamp(targetPosition.x, HorizontalRange.x, HorizontalRange.y);
                    float clampedY = Mathf.Clamp(targetPosition.y, VerticalRange.x, VerticalRange.y);

                    // Update BrushTransform position
                    BrushTransform.anchoredPosition = Vector2.Lerp(BrushTransform.anchoredPosition, new Vector2(clampedX, clampedY), _speed * Time.deltaTime);

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
                }

                if (Input.GetMouseButtonUp(0))
                {
                    initialOffset = Vector2.zero;
                    InfoText.gameObject.SetActive(true);
                }
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

        void StartColoring()
        {
            _partTexture = ColoringPart[_paintingCounter].GetCurrenTexture();
            ColoringImage.DOAnchorPos(Vector2.zero, _preparationTime).OnComplete(()=> {
                RefferanceImg.sprite = RefferanceSprites[_paintingCounter];
                PaintBrush.SetActive(true);
                InfoText.gameObject.SetActive(true);
                RefferanceBar.SetActive(true);
            });
        }

        void ColorSelected()
        {
            InfoText.text = InfoMsgs[1];
            if (_movingRoutine == null)
            {
                _canColor = true;
                _movingRoutine = StartCoroutine(MovingRoutine());
            }
        }
    }
}
