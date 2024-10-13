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

        Coroutine _movingRotine;
        Vector2 _brushOffset;
        [SerializeField] float _speed = 5;

        public void OnBeginDrag()
        {
            // Get the initial screen position and calculate the brush offset
            Vector2 screenPosition = Input.mousePosition; 
            RectTransformUtility.ScreenPointToLocalPointInRectangle(BrushTransform, screenPosition, null, out Vector2 localPoint);
            _brushOffset = BrushTransform.localPosition - (Vector3)localPoint;

            // Start the coloring coroutine
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
            Vector2 targetPosition;

            while (IsDragging.Value == 1)
            {
                Vector2 screenPosition = Input.mousePosition;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(BrushTransform, screenPosition, null, out Vector2 localPoint);
                targetPosition = localPoint + _brushOffset;
                BrushTransform.localPosition = Vector2.Lerp(BrushTransform.localPosition, targetPosition, _speed * Time.deltaTime);
                BurshPosition.Value = BrushTransform.localPosition;
                yield return new WaitForSeconds(waiting);
            }
        }
    }
}
