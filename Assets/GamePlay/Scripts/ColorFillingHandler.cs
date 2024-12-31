using TMPro;
using UnityEngine;
using DG.Tweening;
using Core.Events;
using Core.Variables;
using System.Collections;

namespace Core.GamePlay.Coloring
{
    public class ColorFillingHandler : MonoBehaviour
    {
        [SerializeField] SOIntegerEvents LoopEffectEvent;
        [SerializeField] SOEvents StartColoringEvent, ColorSelectedEvent, StopLoopEffect;
        [SerializeField] SOColor CurrentColor;
        [SerializeField] ColorFilling[] ColoringPart;
        [SerializeField] RectTransform BrushTransform;
        [SerializeField] Vector2Int VerticalRange, HorizontalRange;
        [SerializeField] TextMeshProUGUI InfoText;
        [SerializeField] string[] InfoMsgs;
        [SerializeField] GameObject PaintBrush, NextBtn, TouchProtector, DetailsImg;
        [SerializeField] Animation BrushAnimtion;
        [SerializeField] ParticleSystem BrushParitcle;
        [SerializeField] ParticleSystemRenderer BrushParitcleRenderrer;

        float _speed = 5, _brushSize = 15, _preparationTime = 1;
        bool _canColor = false, _coloringSound = false, _canShowNextBtn = true, _onceClicked = true;
        Coroutine _movingRoutine;
        RectTransform _coloringParTransform;
        Camera _currentCamera;
        Texture2D _partTexture;
        int _paintingCounter = 0, _totalColorPixles = 1, _coloredPixlesCounter = 0;
        Color32[] _partPixles;
        Color _whiteColor = Color.white;
        MaterialPropertyBlock _materialPropertyBlock;

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
            _materialPropertyBlock = new MaterialPropertyBlock();
        }

        void StartColoring()
        {
            TouchProtector.SetActive(true);
            _partTexture = ColoringPart[_paintingCounter].GetCurrenTexture();
            _totalColorPixles = ColoringPart[_paintingCounter].GetColoredPixlesCount();
            _partPixles = _partTexture.GetPixels32();
            PaintBrush.SetActive(true);
            PaintBrush.transform.DOMoveX(0, _preparationTime).SetEase(Ease.InBack).OnComplete(() =>
            {
                InfoText.gameObject.SetActive(true);
            });
        }

        void ColorSelected()
        {
            InfoText.text = InfoMsgs[1];
            _materialPropertyBlock.SetColor("_BaseColor", CurrentColor.Value);
            BrushParitcleRenderrer.SetPropertyBlock(_materialPropertyBlock);
            ParticleSystem.MainModule pm = BrushParitcle.main;
            Color newColor = CurrentColor.Value;
            pm.startColor = newColor;
            if (_movingRoutine == null)
            {
                _canColor = true;
                _movingRoutine = StartCoroutine(BrushMovingRoutine());
                TouchProtector.SetActive(false);
                BrushParitcle.gameObject.SetActive(true);
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
                    if (!_coloringSound)
                    {
                        _coloringSound = true;
                        BrushAnimtion.Play();
                        LoopEffectEvent.InvokeSOEvent(0);
                    }
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
                    BrushAnimtion.Stop();
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
                    TouchProtector.SetActive(false);
                    _canShowNextBtn = true;
                }
                else 
                {
                    DetailsImg.SetActive(true);
                }
            }
        }
    }
}
