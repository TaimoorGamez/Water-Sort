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
        [SerializeField] RectTransform BrushTransform, ColoringImage, SprayCan, FlameThrower;
        [SerializeField] Vector2Int VerticalRange, HorizontalRange;
        [SerializeField] TextMeshProUGUI InfoText;
        [SerializeField] string[] InfoMsgs;
        [SerializeField] GameObject PaintBrush, RefferanceBar, NextBtn, TouchProtector, DetailsImg;
        [SerializeField] Image RefferanceImg, Details;
        [SerializeField] RawImage CompoundImg;
        [SerializeField] Sprite[] RefferanceSprites;
        [SerializeField] Texture2D CloudTexture;
        [SerializeField] ParticleSystem BubbleParticle, StarParticles, FlameParticles;

        float _speed = 5, _brushSize = 15, _preparationTime = 1, _finalPos = 100, _finalScale = 1.5f, _infoTextPos = 250f;
        bool _canColor = false, _coloringSound = false, _canShowNextBtn = true, _onceClicked = true,
             _detailsApplied = false, _canThrowFlame = false;
        Coroutine _movingRoutine;
        RectTransform _coloringParTransform;
        Camera _currentCamera;
        Texture2D _partTexture;
        int _paintingCounter = 0, _totalColorPixles = 0, _coloredPixlesCounter = 0;
        Color32[] _cloudPixles, _compoundPixels, _partPixles;
        List<Vector2Int> _brushCircle;
        Color _whiteColor = Color.white;

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
            _brushCircle = GenerateBrushCircle(Mathf.RoundToInt(_brushSize));
        }

        void StartColoring()
        {
            TouchProtector.SetActive(true);
            _partTexture = ColoringPart[_paintingCounter].GetCurrenTexture();
            _totalColorPixles = ColoringPart[_paintingCounter].GetColoredPixlesCount();
            _partPixles = _partTexture.GetPixels32();
            SoundEffectEvent.InvokeSOEvent(3);
            ColoringImage.DOAnchorPos(Vector2.zero, _preparationTime).OnComplete(() =>
            {
                RefferanceImg.sprite = RefferanceSprites[_paintingCounter];
                PaintBrush.SetActive(true);
                InfoText.gameObject.SetActive(true);
                RefferanceBar.SetActive(true);
            });
        }

        List<Vector2Int> GenerateBrushCircle(int radius)
        {
            List<Vector2Int> circlePixels = new List<Vector2Int>();
            int radiusSquared = radius * radius;

            for (int y = -radius; y <= radius; y++)
            {
                for (int x = -radius; x <= radius; x++)
                {
                    if (x * x + y * y <= radiusSquared)
                    {
                        circlePixels.Add(new Vector2Int(x, y));
                    }
                }
            }

            return circlePixels;
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
                    _coloringSound = false;
                    initialOffset = Vector2.zero;
                    if(_canShowNextBtn)
                       InfoText.gameObject.SetActive(true);
                }
                yield return null; // Yield until the next frame
            }
        }


        void ApplyBrush(int centerX, int centerY)
        {
            int brushWidth = Mathf.RoundToInt(_brushSize * 2);
            int brushHeight = Mathf.RoundToInt(_brushSize * 2);
            int compoundWidth = _partTexture.width;
            int compoundHeight = _partTexture.height;

            // Calculate bounds of the brush area on the compound texture
            int startX = centerX - brushWidth / 2;
            int startY = centerY - brushHeight / 2;

            // Loop through the brush area
            for (int y = 0; y < brushHeight; y++)
            {
                // Calculate the target Y index once per row
                int targetY = startY + y;

                // Only process if the targetY is within the compound texture bounds
                if (targetY >= 0 && targetY < compoundHeight)
                {
                    for (int x = 0; x < brushWidth; x++)
                    {
                        int targetX = startX + x;

                        // Only process if the targetX is within the compound texture bounds
                        if (targetX >= 0 && targetX < compoundWidth)
                        {
                            int brushIndex = y * brushWidth + x;
                            int compoundIndex = targetY * compoundWidth + targetX;

                            // Blend only if the brush pixel is within the circular area
                            float distSquared = (x - brushWidth / 2) * (x - brushWidth / 2) + (y - brushHeight / 2) * (y - brushHeight / 2);
                            if (distSquared <= _brushSize * _brushSize && _partPixles[compoundIndex] == _whiteColor)
                            {
                                _partPixles[compoundIndex] = CurrentColor.Value;
                                _coloredPixlesCounter++;
                                if (!_coloringSound)
                                {
                                    _coloringSound = true;
                                    LoopEffectEvent.InvokeSOEvent(0);
                                }
                            }
                        }
                    }
                }
            }

            // Apply the updated pixels back to the texture
            _partTexture.SetPixels32(_partPixles);
            _partTexture.Apply();
            float totalFilling = ((float)_coloredPixlesCounter / _totalColorPixles) * 100;
            if (totalFilling >= 95 && _canShowNextBtn)
            {
                _canShowNextBtn = false;
                _onceClicked = false;
                NextBtn.SetActive(true);
                _paintingCounter++;
            }
        }

        void FillRemaingPixles()
        {
            if (_coloredPixlesCounter < _totalColorPixles)
            {
                for (int p = 0; p < _partPixles.Length; p++)
                {
                    if (_partPixles[p] == _whiteColor)
                    {
                        _partPixles[p] = CurrentColor.Value;
                    }
                }
                _partTexture.SetPixels32(_partPixles);
                _partTexture.Apply();
            }
        }

        public void OnNextBtnClick()
        {
            if (!_onceClicked)
            {
                _onceClicked = true;
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
                    _totalColorPixles = ColoringPart[_paintingCounter].GetColoredPixlesCount();
                    _partPixles = _partTexture.GetPixels32();
                    InfoText.text = InfoMsgs[0];
                    PaintBrush.SetActive(true);
                    InfoText.gameObject.SetActive(true);
                    RefferanceImg.sprite = RefferanceSprites[_paintingCounter];
                    TouchProtector.SetActive(false);
                    _canShowNextBtn = true;
                }
                else 
                {
                    DetailsImg.SetActive(true);
                }
            }
        }

        //-----------------------------------------
        //public Camera screenshotCamera; // Assign your screenshot camera here
        //public RenderTexture renderTexture; // Assign your RenderTexture here
        //public RawImage displayImage; // Assign the RawImage in your complete panel

        //public void CaptureColoredArea()
        //{
        //    // Set the camera to render the colored area
        //    screenshotCamera.targetTexture = renderTexture;
        //    screenshotCamera.Render();

        //    // Create a new Texture2D
        //    Texture2D screenshot = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);

        //    // Read the pixels from the RenderTexture
        //    RenderTexture.active = renderTexture;
        //    screenshot.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
        //    screenshot.Apply();

        //    // Reset the RenderTexture
        //    RenderTexture.active = null;
        //    screenshotCamera.targetTexture = null;

        //    // Display the screenshot in the complete panel
        //    displayImage.texture = screenshot;

        //    // Optional: Save the screenshot as a PNG
        //    byte[] bytes = screenshot.EncodeToPNG();
        //    System.IO.File.WriteAllBytes(Application.persistentDataPath + "/ColoredArea.png", bytes);
        //    Debug.Log("Screenshot saved to: " + Application.persistentDataPath + "/ColoredArea.png");
        //}
    }
}
