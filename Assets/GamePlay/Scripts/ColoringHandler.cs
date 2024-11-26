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
        [SerializeField] RectTransform BrushTransform, ColoringImage, SprayCan;
        [SerializeField] Vector2Int VerticalRange, HorizontalRange;
        [SerializeField] TextMeshProUGUI InfoText;
        [SerializeField] string[] InfoMsgs;
        [SerializeField] GameObject PaintBrush, RefferanceBar, NextBtn, TouchProtector;
        [SerializeField] Image RefferanceImg;
        [SerializeField] RawImage CompoundImg;
        [SerializeField] Sprite[] RefferanceSprites;
        [SerializeField] Texture2D BubbleTexture;

        float _speed = 5, _brushSize = 15, _preparationTime = 1, _finalPos = 100, _finalScale = 1.5f, _infoTextPos = -365f;
        bool _canColor = false, _canSpray;
        Coroutine _movingRoutine;
        RectTransform _coloringParTransform;
        Camera _currentCamera;
        Texture2D _partTexture;
        int _paintingCounter = 0, _totalPixles = 0, _compoundTextureLength = 500;
        Vector2Int _lastBrushPos = new Vector2Int(-1, -1);
        List<int> _coloredPixels = new List<int>();
        Color32[] _bubblePixles;

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

        IEnumerator BrushMovingRoutine()
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

                if (Input.GetMouseButtonUp(0) && !NextBtn.activeInHierarchy)
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
                _paintingCounter++;
            }
        }



        void StartColoring()
        {
            TouchProtector.SetActive(true);
            _partTexture = ColoringPart[_paintingCounter].GetCurrenTexture();
            _coloredPixels = ColoringPart[_paintingCounter].GetColoredPixles();
            _totalPixles = _coloredPixels.Count;
            ColoringImage.DOAnchorPos(Vector2.zero, _preparationTime).OnComplete(() =>
            {
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
                _movingRoutine = StartCoroutine(BrushMovingRoutine());
                TouchProtector.SetActive(false);
            }
        }

        public void OnNextBtnClick()
        {
            TouchProtector.SetActive(true);
            NextBtn.SetActive(false);
            _canColor = false; 
            PaintBrush.SetActive(false);
            if (_movingRoutine != null)
            {
                StopCoroutine(_movingRoutine);
                _movingRoutine = null;
            }
            InfoText.gameObject.SetActive(false);
            FillRemaingPixles();
            if (_paintingCounter >= ColoringPart.Length)
            {
                PrepareCompounD();
                InfoText.text = InfoMsgs[2];
                InfoText.gameObject.SetActive(true);
                InfoText.rectTransform.DOAnchorPosY(_infoTextPos, _preparationTime).OnComplete(()=> {
                    SprayCan.gameObject.SetActive(true);
                    _canSpray = true;
                    _movingRoutine = StartCoroutine(SprayMovingRoutine());
                });
            }
            else
            {
                _partTexture = ColoringPart[_paintingCounter].GetCurrenTexture();
                _coloredPixels = ColoringPart[_paintingCounter].GetColoredPixles();
                InfoText.text = InfoMsgs[0];
                PaintBrush.SetActive(true);
                InfoText.gameObject.SetActive(true);
                _totalPixles = _coloredPixels.Count;
                RefferanceImg.sprite = RefferanceSprites[_paintingCounter];
            }
        }

        void FillRemaingPixles()
        {
            if (_coloredPixels.Count > 0)
            {
                // Get all the pixels of the texture
                Color32[] pixels = _partTexture.GetPixels32();

                // Iterate through the remaining uncolored pixels
                foreach (int pixelIndex in _coloredPixels)
                {
                    // Set the pixel color to the current color
                    pixels[pixelIndex] = CurrentColor.Value;
                }

                // Apply the updated pixels back to the texture
                _partTexture.SetPixels32(pixels);
                _partTexture.Apply();

                // Once all remaining pixels are filled, clear the list of colored pixels
                _coloredPixels.Clear();
            }
        }

        void PrepareCompounD()
        {
            _partTexture = new Texture2D(_compoundTextureLength, _compoundTextureLength, TextureFormat.RGBA32, false); 
            Color32[] pixels = _partTexture.GetPixels32();

            // Example: Set all pixels to a light color (you can customize this as needed)
            Color32 fillColor = new Color32(0, 0, 0, 0); // Red color for example

            // Apply the color to all pixels
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = fillColor;
            }

            // Apply the updated pixels to the texture
            _partTexture.SetPixels32(pixels);
            _partTexture.Apply();
            CompoundImg.texture = _partTexture;
            CompoundImg.gameObject.SetActive(true);
            _bubblePixles = BubbleTexture.GetPixels32();
        }


        IEnumerator SprayMovingRoutine()
        {
            Vector2 initialOffset = Vector2.zero;
            float smoothTimer = 0.01f;

            while (_canSpray)
            {
                //if (Input.GetMouseButtonDown(0))
                //{
                //    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                //        _coloringParTransform, Input.mousePosition, _currentCamera, out Vector2 initialPoint);
                //    initialOffset = SprayCan.anchoredPosition - initialPoint;
                //    InfoText.gameObject.SetActive(false);
                //}

                //if (Input.GetMouseButton(0))
                //{
                //    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                //        _coloringParTransform, Input.mousePosition, _currentCamera, out Vector2 localPoint);

                //    // Calculate and clamp target position
                //    Vector2 targetPosition = localPoint + initialOffset;
                //    targetPosition.x = Mathf.Clamp(targetPosition.x, HorizontalRange.x, HorizontalRange.y);
                //    targetPosition.y = Mathf.Clamp(targetPosition.y, VerticalRange.x, VerticalRange.y);

                //    // Smoothly move BrushTransform to the target position
                //    SprayCan.anchoredPosition = Vector2.Lerp(
                //        SprayCan.anchoredPosition, targetPosition, _speed * Time.deltaTime);

                //    // Convert to UV coordinates and apply the brush
                //    Vector2 brushUV = new Vector2(
                //        (SprayCan.anchoredPosition.x - _coloringParTransform.rect.x) / _coloringParTransform.rect.width,
                //        (SprayCan.anchoredPosition.y - _coloringParTransform.rect.y) / _coloringParTransform.rect.height);

                //    int texX = Mathf.RoundToInt(brushUV.x * _partTexture.width);
                //    int texY = Mathf.RoundToInt(brushUV.y * _partTexture.height);
                //}

                //if (Input.GetMouseButtonUp(0) && !NextBtn.activeInHierarchy)
                //{
                //    initialOffset = Vector2.zero;
                //    InfoText.gameObject.SetActive(true);
                //}

                yield return new WaitForSeconds(smoothTimer); // Yield until the next frame
            }
        }

    }
}
