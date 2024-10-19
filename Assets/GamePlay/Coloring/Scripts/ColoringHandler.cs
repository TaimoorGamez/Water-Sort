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
        [SerializeField] Vector2Int VerticalRange, HorizontalRange; // Use these for boundary clamping

        float _speed = 500;
        Coroutine _movingRoutine;
        RectTransform _canvasTransform;
        Camera _currentCamera;

        private void Start()
        {
            _currentCamera = Camera.main;
            _canvasTransform = GetComponentInParent<Canvas>().transform as RectTransform;
        }

        public void OnBeginDrag()
        {
            // Start the coloring coroutine
            IsDragging.Value = 1;
            _movingRoutine = StartCoroutine(MovingRoutine());
            ColoringPart.StartColoring();
        }

        public void OnEndDrag()
        {
            IsDragging.Value = 0;
            ColoringPart.StopColoring();
            if (_movingRoutine != null)
            {
                StopCoroutine(_movingRoutine);
            }
        }

        IEnumerator MovingRoutine()
        {
            float waiting = 0.01f;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasTransform, Input.mousePosition, _currentCamera, out Vector2 initialPoint);
            Vector2 initialOffset = BrushTransform.anchoredPosition - initialPoint;

            while (IsDragging.Value == 1)
            {
                // Convert screen position to local position relative to the RectTransform
                RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasTransform, Input.mousePosition, _currentCamera, out Vector2 localPoint);


                // Calculate target position with initial offset
                Vector2 targetPosition = localPoint + initialOffset;

                // Clamp the position within defined boundaries
                float clampedX = Mathf.Clamp(targetPosition.x, HorizontalRange.x, HorizontalRange.y);
                float clampedY = Mathf.Clamp(targetPosition.y, VerticalRange.x, VerticalRange.y);

                // Update only X and Y positions while keeping Z unchanged
                BrushTransform.anchoredPosition = Vector2.MoveTowards(BrushTransform.anchoredPosition, new Vector2(clampedX, clampedY), _speed * Time.deltaTime);

                yield return new WaitForSeconds(waiting);
            }
        }

    }
}
