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
        [SerializeField] SOIntegerEvents LoopEffectEvent, SoundEffectEvent;
        [SerializeField] SOEvents StartColoringEvent, ColorSelectedEvent, StopLoopEffect;
        [SerializeField] SOColor CurrentColor;
        [SerializeField] ColorFilling[] ColoringPart;
        [SerializeField] RectTransform BrushTransform, ColoringImage, SprayCan;
        [SerializeField] Vector2Int VerticalRange, HorizontalRange;
        [SerializeField] TextMeshProUGUI InfoText;
        [SerializeField] string[] InfoMsgs;
        [SerializeField] GameObject PaintBrush, RefferanceBar, NextBtn, TouchProtector;
        [SerializeField] Image RefferanceImg, Details;
        [SerializeField] RawImage CompoundImg;
        [SerializeField] Sprite[] RefferanceSprites;
        [SerializeField] Texture2D BubbleTexture;
        [SerializeField] ParticleSystem BubbleParticle;

        float _speed = 5, _brushSize = 15, _preparationTime = 1, _finalPos = 100, _finalScale = 1.5f, _infoTextPos = -365f;
        bool _canColor = false, _canSpray, _detailsAplied = false, _effectCheck = false, _canShowNextBtn = true;
        Coroutine _movingRoutine;
        RectTransform _coloringParTransform;
        Camera _currentCamera;
        Texture2D _partTexture;
        int _paintingCounter = 0, _totalColorPixles = 0, _compoundTextureLength = 500, _lastAppliedCenterY = -1, _lastAppliedCenterX = -1, _totalBubbles = 50, _bubbleCounter = 0;
        Vector2Int _lastBrushPos = new Vector2Int(-1, -1);
        List<int> _coloredPixels = new List<int>();
        Color32[] _bubblePixles, _compoundPixels; 
        const int _bubbleThreshold = 35;

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

                if (Input.GetMouseButtonUp(0))
                {
                    StopLoopEffect.InvokeSOEvent();
                    _effectCheck = false;
                    initialOffset = Vector2.zero;
                    if(_canShowNextBtn)
                       InfoText.gameObject.SetActive(true);
                }
                Debug.Log("1st");
                yield return null; // Yield until the next frame
            }
        }


        void ApplyBrush(int centerX, int centerY)
        {
            int brushRadius = Mathf.RoundToInt(_brushSize);

            // Only update if the brush has moved significantly
            if (_lastBrushPos == new Vector2Int(centerX, centerY))
            {
                StopLoopEffect.InvokeSOEvent();
                _effectCheck = false;
                return;
            }

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
                            if (!_effectCheck)
                            {
                                _effectCheck = true;
                                LoopEffectEvent.InvokeSOEvent(0);
                            }
                            pixels[pixelIndex] = CurrentColor.Value;
                            _coloredPixels.Remove(pixelIndex);
                        }
                    }
                }
            }

            // Apply the updated pixels back to the texture
            _partTexture.SetPixels32(pixels);
            _partTexture.Apply();

            float remainingPercentage = (_coloredPixels.Count / (float)_totalColorPixles) * 100;
            if (remainingPercentage <= 5 && _canShowNextBtn)
            {
                _canShowNextBtn = false;
                NextBtn.SetActive(true);
                _paintingCounter++;
            }
        }



        void StartColoring()
        {
            TouchProtector.SetActive(true);
            _partTexture = ColoringPart[_paintingCounter].GetCurrenTexture();
            _coloredPixels = ColoringPart[_paintingCounter].GetColoredPixles();
            _totalColorPixles = _coloredPixels.Count;
            SoundEffectEvent.InvokeSOEvent(3);
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
            _canColor = false;
            TouchProtector.SetActive(true);
            PaintBrush.SetActive(false);
            if (_movingRoutine != null)
            {
                StopCoroutine(_movingRoutine);
                _movingRoutine = null;
            }
            InfoText.gameObject.SetActive(false);
            NextBtn.SetActive(false);
            FillRemaingPixles();

         if (_paintingCounter < ColoringPart.Length)
            {
                _partTexture = ColoringPart[_paintingCounter].GetCurrenTexture();
                _coloredPixels = ColoringPart[_paintingCounter].GetColoredPixles();
                InfoText.text = InfoMsgs[0];
                PaintBrush.SetActive(true);
                InfoText.gameObject.SetActive(true);
                _totalColorPixles = _coloredPixels.Count;
                RefferanceImg.sprite = RefferanceSprites[_paintingCounter];
                TouchProtector.SetActive(false);
                _canShowNextBtn = true;
            }
            else if (_paintingCounter >= ColoringPart.Length && !_detailsAplied)
            {
                PrepareCompounD();
                InfoText.text = InfoMsgs[2];
                InfoText.gameObject.SetActive(true);
                InfoText.rectTransform.DOAnchorPosY(_infoTextPos, _preparationTime).OnComplete(() =>
                {
                    SprayCan.gameObject.SetActive(true);
                    _canSpray = true;
                    _movingRoutine = StartCoroutine(SprayMovingRoutine());
                    TouchProtector.SetActive(false);
                    _canShowNextBtn = true;
                });
            }
            else if(_detailsAplied)
            {
                SprayCan.gameObject.SetActive(false);
                CompoundImg.rectTransform.DOScale(Vector3.zero,_preparationTime).SetEase(Ease.InBack).OnComplete(()=> {
                    Details.gameObject.SetActive(true);
                    Details.DOFillAmount(1, _preparationTime);
                    ColoringImage.DOScale(_finalScale, _preparationTime);
                    ColoringImage.DOAnchorPosY(_finalPos, _preparationTime);
                });
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
            _compoundPixels = _partTexture.GetPixels32();

            // Example: Set all pixels to a light color (you can customize this as needed)
            Color32 fillColor = new Color32(0, 0, 0, 0); // Red color for example

            // Apply the color to all pixels
            for (int i = 0; i < _compoundPixels.Length; i++)
            {
                _compoundPixels[i] = fillColor;
            }
            // Apply the updated pixels to the texture
            _partTexture.SetPixels32(_compoundPixels);
            _partTexture.Apply();
            CompoundImg.texture = _partTexture;
            CompoundImg.gameObject.SetActive(true);
            _bubblePixles = BubbleTexture.GetPixels32();
        }


        IEnumerator SprayMovingRoutine()
        {
            Vector2 initialOffset = Vector2.zero;
            float smoothTimer = 0.01f;
            RectTransform coloringParTransform = CompoundImg.rectTransform;

            while (_canSpray)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        coloringParTransform, Input.mousePosition, _currentCamera, out Vector2 initialPoint);
                    initialOffset = SprayCan.anchoredPosition - initialPoint;
                    InfoText.gameObject.SetActive(false);
                    BubbleParticle.Play();
                }

                if (Input.GetMouseButton(0))
                {
                    RectTransformUtility.ScreenPointToLocalPointInRectangle(
                        coloringParTransform, Input.mousePosition, _currentCamera, out Vector2 localPoint);

                    // Calculate and clamp target position
                    Vector2 targetPosition = localPoint + initialOffset;
                    targetPosition.x = Mathf.Clamp(targetPosition.x, HorizontalRange.x, HorizontalRange.y);
                    targetPosition.y = Mathf.Clamp(targetPosition.y, VerticalRange.x, VerticalRange.y);

                    // Smoothly move BrushTransform to the target position
                    SprayCan.anchoredPosition = Vector2.Lerp(
                        SprayCan.anchoredPosition, targetPosition, _speed * Time.deltaTime);

                    Vector2 brushUV = new Vector2(
                    (SprayCan.anchoredPosition.x - coloringParTransform.rect.x) / coloringParTransform.rect.width,
                    (SprayCan.anchoredPosition.y - coloringParTransform.rect.y) / coloringParTransform.rect.height);

                    int texX = Mathf.RoundToInt(brushUV.x * _partTexture.width);
                    int texY = Mathf.RoundToInt(brushUV.y * _partTexture.height);

                    ApplyBubble(texX, texY);
                }

                if (Input.GetMouseButtonUp(0))
                {
                    initialOffset = Vector2.zero;
                    BubbleParticle.Stop();
                    if (_canShowNextBtn)
                    InfoText.gameObject.SetActive(true);
                }
                Debug.Log("2nd");
                yield return new WaitForSeconds(smoothTimer); // Yield until the next frame
            }
        }

        void ApplyBubble(int centerX, int centerY)
        {
            // Check if the position has changed significantly
            if (Mathf.Abs(centerX - _lastAppliedCenterX) <= _bubbleThreshold && Mathf.Abs(centerY - _lastAppliedCenterY) <= _bubbleThreshold)
            {
                return; // Skip applying the bubble if the position hasn't changed enough
            }

            _bubbleCounter++;
            // Save the new position as the last applied position
            _lastAppliedCenterX = centerX;
            _lastAppliedCenterY = centerY;

            int bubbleWidth = BubbleTexture.width;
            int bubbleHeight = BubbleTexture.height;
            int compoundWidth = _partTexture.width;
            int compoundHeight = _partTexture.height;

            // Calculate bounds of the bubble on the compound texture
            int startX = centerX - bubbleWidth / 2;
            int startY = centerY - bubbleHeight / 2;

            // Loop through bubble texture pixels
            for (int y = 0; y < bubbleHeight; y++)
            {
                // Calculate the target Y index once per row
                int targetY = startY + y;

                // Only process if the targetY is within the compound texture bounds
                if (targetY >= 0 && targetY < compoundHeight)
                {
                    for (int x = 0; x < bubbleWidth; x++)
                    {
                        int targetX = startX + x;

                        // Only process if the targetX is within the compound texture bounds
                        if (targetX >= 0 && targetX < compoundWidth)
                        {
                            int bubbleIndex = y * bubbleWidth + x;
                            int compoundIndex = targetY * compoundWidth + targetX;

                            // Blend only if the bubble pixel is not fully transparent
                            if (_bubblePixles[bubbleIndex].a > 0)
                            {
                                _compoundPixels[compoundIndex] = _bubblePixles[bubbleIndex];
                            }

                           
                        }
                    }
                }
            }

            // Set all the updated pixels at once
            _partTexture.SetPixels32(_compoundPixels);
            _partTexture.Apply();
            if (_bubbleCounter >= _totalBubbles && _canShowNextBtn)
            {
                Debug.Log("line 414");
                _canShowNextBtn = false;
                   _detailsAplied = true;
                NextBtn.SetActive(true);
            }
        }


        //-----------------------------------------
        public Camera screenshotCamera; // Assign your screenshot camera here
        public RenderTexture renderTexture; // Assign your RenderTexture here
        public RawImage displayImage; // Assign the RawImage in your complete panel

        public void CaptureColoredArea()
        {
            // Set the camera to render the colored area
            screenshotCamera.targetTexture = renderTexture;
            screenshotCamera.Render();

            // Create a new Texture2D
            Texture2D screenshot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);

            // Read the pixels from the RenderTexture
            RenderTexture.active = renderTexture;
            screenshot.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            screenshot.Apply();

            // Reset the RenderTexture
            RenderTexture.active = null;
            screenshotCamera.targetTexture = null;

            // Display the screenshot in the complete panel
            displayImage.texture = screenshot;

            // Optional: Save the screenshot as a PNG
            byte[] bytes = screenshot.EncodeToPNG();
            System.IO.File.WriteAllBytes(Application.persistentDataPath + "/ColoredArea.png", bytes);
            Debug.Log("Screenshot saved to: " + Application.persistentDataPath + "/ColoredArea.png");
        }
    }
}
