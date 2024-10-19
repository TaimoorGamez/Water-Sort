using UnityEngine;
using Core.Variables;
using System.Collections;

namespace Core.GamePlay.Coloring
{
    public class ColoringHandler : MonoBehaviour
    {
        [SerializeField] SOInterger IsDragging;
        [SerializeField] SOVector BurshPosition;
        [SerializeField] ColorFilling ColoringPart;
        [SerializeField] RectTransform BrushTransform;
        [SerializeField] Vector2Int VerticalRange, HorizontalRange;

        Coroutine _movingRotine;
        Vector2 _brushOffset;
        [SerializeField] float _speed = 5;
        Camera _currentCamera;

        public void OnBeginDrag()
        {
            // Get the initial screen position and calculate the brush offset
            Vector2 screenPosition = Input.mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(BrushTransform, screenPosition, null, out Vector2 localPoint);
            _brushOffset = BrushTransform.localPosition - (Vector3)localPoint;

            // Start the coloring coroutine
            _currentCamera = Camera.main;
            IsDragging.Value = 1;
            _movingRotine = StartCoroutine(MovingCorotine());
            ColoringPart.StartColoring();
        }

        public void OnEndDrag()
        {
            IsDragging.Value = 0;
            ColoringPart.StopColoring();
            if (_movingRotine != null)
            {
                StopCoroutine(_movingRotine);
            }
        }

        IEnumerator MovingCorotine()
        {
            float waiting = 0.01f;
            Vector2 lastMousePosition = Input.mousePosition; // Store the initial mouse position
            Vector2 initialBrushPosition = BrushTransform.localPosition; // Store the initial brush position

            // Calculate the initial offset between the brush and the mouse
            Vector2 initialOffset = initialBrushPosition - lastMousePosition;

            while (IsDragging.Value == 1)
            {
                // Get the current mouse position
                Vector2 currentMousePosition = Input.mousePosition;

                // Calculate the distance the mouse has moved
                Vector2 mouseDelta = currentMousePosition - lastMousePosition;
                lastMousePosition = currentMousePosition; // Update the last mouse position

                // Calculate the new position for the brush based on the mouse movement (delta)
                Vector2 targetBrushPosition = BrushTransform.localPosition + (Vector3)mouseDelta;

                // Maintain the initial offset between the brush and the mouse
                targetBrushPosition = currentMousePosition - initialOffset;

                // Check screen boundaries to ensure the brush stays within the screen
                Vector2 clampedPosition = new Vector2(
                    Mathf.Clamp(targetBrushPosition.x, HorizontalRange.x, HorizontalRange.y),
                    Mathf.Clamp(targetBrushPosition.y, VerticalRange.x, VerticalRange.y)
                );

                // Move the brush smoothly towards the new clamped position
                BrushTransform.localPosition = Vector3.MoveTowards(BrushTransform.localPosition, clampedPosition, _speed * Time.deltaTime);

                // Update the brush position variable
                BurshPosition.Value = BrushTransform.localPosition;

                yield return new WaitForSeconds(waiting);
            }
        }

    }
}
