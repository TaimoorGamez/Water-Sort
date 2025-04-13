using DG.Tweening;
using UnityEngine;
using Core.Variables;

namespace Core.Screen
{
    public class MultiplyBarHandler : MonoBehaviour
    {
        [SerializeField] SOInterger MultiplyerCounter;
        [SerializeField] RectTransform selector;
        [SerializeField] RectTransform[] multiplierAnchors;

        float _moveDuration = 0.25f;
        Sequence _moveSequence;

        private void Start()
        {
            selector.anchoredPosition = multiplierAnchors[0].anchoredPosition;
            MoveAnchor();
        }

        void MoveAnchor()
        {

            _moveSequence = DOTween.Sequence().SetLoops(-1, LoopType.Yoyo);

            for (int i = 0; i < multiplierAnchors.Length; i++)
            {
                _moveSequence.Append(
                    selector.DOAnchorPos(multiplierAnchors[i].anchoredPosition, _moveDuration)
                            .SetEase(Ease.InOutSine))
                    .AppendCallback(() => MultiplyerCounter.Value = GetMultiplierFromIndex(i));
            }
        }

        int GetMultiplierFromIndex(int index)
        {
            switch (index)
            {
                case 0: return 2;
                case 1: return 3;
                case 2: return 5;
                case 3: return 3;
                case 4: return 2;
                default: return 1;
            }
        }

        private void OnDisable()
        {
            _moveSequence.Kill();
        }
    }
}
