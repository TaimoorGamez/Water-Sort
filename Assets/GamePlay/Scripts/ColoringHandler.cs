using TMPro;
using UnityEngine;
using DG.Tweening;
using Core.Events;
using Core.Variables;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

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
        [SerializeField] GameObject PaintBrush, RefferanceBar, NextBtn;
        [SerializeField] Image RefferanceImg;
        [SerializeField] Sprite[] RefferanceSprites;

        float _speed = 5, _brushSize = 15, _alphaThreshold = 0.1f, _preparationTime = 1, _finalPos = 100, _finalScale = 1.5f;
        bool _canColor = false;
        Coroutine _movingRoutine;
        RectTransform _coloringParTransform;
        Camera _currentCamera;
        Texture2D _partTexture;
        int _paintingCounter = 0, _totalPixles = 0;
        Vector2Int _lastBrushPos = new Vector2Int(-1, -1);
        List<int> _coloredPixels = new List<int>();

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
            Vector2 initialOffset = Vector2.zero;

            while (_canColor)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        _coloringParTransform, Input.mousePosition, _currentCamera, out Vector2 initialPoint);
                    initialOffset = BrushTransform.anchoredPosition - initialPoint;
                    InfoText.gameObject.SetActive(false);
                }

                if (Input.GetMouseButton(0))
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        _coloringParTransform, Input.mousePosition, _currentCamera, out Vector2 localPoint);

                    // Calculate and clamp target position
                    Vector2 targetPosition = localPoint + initialOffset;
                    targetPosition.x = Mathf.Clamp(targetPosition.x, HorizontalRange.x, HorizontalRange.y);
                    targetPosition.y = Mathf.Clamp(targetPosition.y, VerticalRange.x, VerticalRange.y);

                    // Smoothly move BrushTransform to the target position
                    BrushTransform.anchoredPosition = Vector2.Lerp(
                        BrushTransform.anchoredPosition, targetPosition, _speed * Time.deltaTime);

                    // Convert to UV coordinates and apply the brush
                    Vector2 brushUV = new Vector2(
                        (BrushTransform.anchoredPosition.x - _coloringParTransform.rect.x) / _coloringParTransform.rect.width,
                        (BrushTransform.anchoredPosition.y - _coloringParTransform.rect.y) / _coloringParTransform.rect.height);

                    int texX = Mathf.RoundToInt(brushUV.x * _partTexture.width);
                    int texY = Mathf.RoundToInt(brushUV.y * _partTexture.height);
                    ApplyBrush(texX, texY);
                }

                if (Input.GetMouseButtonUp(0))
                {
                    initialOffset = Vector2.zero;
                    InfoText.gameObject.SetActive(true);
                }

                yield return null; // Yield until the next frame
            }
        }


        void ApplyBrush(int centerX, int centerY)
        {
            int brushRadius = Mathf.RoundToInt(_brushSize);

            // Only update if the brush has moved significantly
            if (_lastBrushPos == new Vector2Int(centerX, centerY))
                return;

            _lastBrushPos = new Vector2Int(centerX, centerY);

            // Get all the pixels of the texture
            Color32[] pixels = _partTexture.GetPixels32();
            int textureWidth = _partTexture.width;

            int radiusSquared = brushRadius * brushRadius;

            for (int y = -brushRadius; y <= brushRadius; y++)
            {
                for (int x = -brushRadius; x <= brushRadius; x++)
                {
                    int pixelX = centerX + x;
                    int pixelY = centerY + y;

                    // Calculate squared distance to avoid using Mathf.Sqrt
                    int distSquared = x * x + y * y;
                    if (distSquared <= radiusSquared &&
                        pixelX >= 0 && pixelX < _partTexture.width &&
                        pixelY >= 0 && pixelY < _partTexture.height)
                    {
                        // Calculate the pixel index in the array
                        int pixelIndex = pixelY * textureWidth + pixelX;
                        if (_coloredPixels.Contains(pixelIndex))
                        {
                            pixels[pixelIndex] = CurrentColor.Value;
                            _coloredPixels.Remove(pixelIndex);
                        }
                    }
                }
            }

            // Apply the updated pixels back to the texture
            _partTexture.SetPixels32(pixels);
            _partTexture.Apply(); 
            
            float remainingPercentage = (_coloredPixels.Count / (float)_totalPixles) * 100;
            if (remainingPercentage <= 5 && !NextBtn.activeInHierarchy)
            {
                NextBtn.SetActive(true);
            }
        }



        void StartColoring()
        {
            _partTexture = ColoringPart[_paintingCounter].GetCurrenTexture();
            _coloredPixels = ColoringPart[_paintingCounter].GetColoredPixles();
            _totalPixles = _coloredPixels.Count;
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

        public void OnNextBtnClick()
        {

        }
    }
}
