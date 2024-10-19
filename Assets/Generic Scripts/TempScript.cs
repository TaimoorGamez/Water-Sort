using UnityEngine;

public class TempScript : ScriptableObject
{
    //IEnumerator ApplyColorCoroutine()
    //{
    //    Vector2 lastMousePos = Vector2.zero;
    //    Vector2 initialOffset = Vector2.zero;
    //    RectTransform fillImageRectTransform = FillImage.rectTransform;
    //    Vector2 imageOffset = GetImageOffset(OriginalImage.rectTransform, FillImage.canvas);
    //    WaitForSeconds wait = new WaitForSeconds(0.01f);

    //    // Get the canvas dimensions in local coordinates
    //    RectTransform canvasRectTransform = FillImage.canvas.GetComponent<RectTransform>();
    //    Vector2 canvasSize = canvasRectTransform.rect.size;

    //    // Convert screen space to local space
    //    //Vector2 minCanvasPosition = canvasRectTransform.TransformPoint(canvasRectTransform.rect.min);
    //    //Vector2 maxCanvasPosition = canvasRectTransform.TransformPoint(canvasRectTransform.rect.max);

    //    while (_isEnable)
    //    {
    //        if (IsMouseDown.Value)
    //        {
    //            HapticFeedbackEvent.InvokeGameEvent();
    //            ColorSelection.SetActive(false);
    //            Vector3 mousePos = Input.mousePosition;
    //            RectTransformUtility.ScreenPointToLocalPointInRectangle(
    //                OriginalImage.rectTransform,
    //                mousePos,
    //                OriginalImage.canvas.worldCamera,
    //                out Vector2 localMousePos
    //            );

    //            if (lastMousePos == Vector2.zero)
    //            {
    //                _started = true;
    //                initialOffset = localMousePos - (Vector2)TattooMachine.localPosition;
    //            }


    //            lastMousePos = localMousePos;
    //            Vector2 targetPosition = localMousePos - initialOffset;
    //            Vector3 clampedPosition = ClampPosition(targetPosition, canvasSize);

    //            TattooMachine.localPosition = Vector3.Lerp(TattooMachine.localPosition, clampedPosition, Time.deltaTime * MoveSpeed);

    //            Vector2 uvPos = LocalToUV(TattooMachine.anchoredPosition - imageOffset, fillImageRectTransform);
    //            int centerX = Mathf.FloorToInt(uvPos.x * _coloredTexture.width);
    //            int centerY = Mathf.FloorToInt(uvPos.y * _coloredTexture.height);

    //            Color32 targetColor = PixelColor;
    //            Color32[] pixels = _coloredTexture.GetPixels32();
    //            float sqrBrushRadius = BrushSize * BrushSize;

    //            for (int i = Mathf.Max(0, centerX - Mathf.CeilToInt(BrushSize)); i < Mathf.Min(_coloredTexture.width, centerX + Mathf.CeilToInt(BrushSize)); i++)
    //            {
    //                for (int j = Mathf.Max(0, centerY - Mathf.CeilToInt(BrushSize)); j < Mathf.Min(_coloredTexture.height, centerY + Mathf.CeilToInt(BrushSize)); j++)
    //                {
    //                    float sqrDistance = (i - centerX) * (i - centerX) + (j - centerY) * (j - centerY);

    //                    if (sqrDistance <= sqrBrushRadius)
    //                    {
    //                        int index = j * _coloredTexture.width + i;
    //                        if (pixels[index].r != targetColor.r ||
    //                            pixels[index].g != targetColor.g ||
    //                            pixels[index].b != targetColor.b ||
    //                            pixels[index].a != targetColor.a)
    //                        {
    //                            pixels[index] = targetColor;
    //                            _filledPixels++;
    //                            FilledPercentage = (float)_filledPixels / (float)_totalPixels * 100f;

    //                            if (FilledPercentage >= TargetPercentage)
    //                                NextButton.gameObject.SetActive(true);
    //                        }
    //                    }
    //                }
    //            }

    //            _coloredTexture.SetPixels32(pixels);
    //            _coloredTexture.Apply();
    //        }
    //        else
    //        {
    //            lastMousePos = Vector2.zero;
    //        }

    //        yield return wait;
    //    }
    //}

    //private Vector3 ClampPosition(Vector2 targetPosition, Vector2 canvasSize)
    //{
    //    // Convert targetPosition from local to world space
    //    Vector3 clampedPosition = new Vector3(
    //        Mathf.Clamp(targetPosition.x, _minPos.x, _maxPos.x),
    //        Mathf.Clamp(targetPosition.y, _minPos.y, _maxPos.y),
    //        TattooMachine.localPosition.z // Keep the z position as is
    //    );

    //    // Convert clampedPosition back to local space
    //    return clampedPosition;
    //}

    //private Vector2 GetImageOffset(RectTransform rectTransform, Canvas canvas)
    //{
    //    // Get the size of the `OriginalImage` in canvas space
    //    Vector2 size = rectTransform.rect.size;
    //    Vector2 pivot = rectTransform.pivot;
    //    Vector2 anchorMin = rectTransform.anchorMin;
    //    Vector2 anchorMax = rectTransform.anchorMax;
    //    Vector2 position = rectTransform.anchoredPosition;

    //    // Calculate the offset considering pivot and anchor
    //    Vector2 offset = position + new Vector2(
    //        size.x * (pivot.x - 0.5f),
    //        size.y * (pivot.y - 0.5f)
    //    );

    //    return offset;
    //}

    //private Vector2 LocalToUV(Vector2 localPosition, RectTransform rectTransform)
    //{
    //    // Convert local position to UV coordinates in the texture
    //    Vector2 uvPos = new Vector2(
    //        (localPosition.x / rectTransform.rect.width) + 0.5f,
    //        (localPosition.y / rectTransform.rect.height) + 0.5f
    //    );
    //}
}