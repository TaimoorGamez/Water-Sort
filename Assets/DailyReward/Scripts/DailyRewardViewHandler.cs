using DG.Tweening;
using UnityEngine;
using Core.Events;
using Core.DB.Variables;

namespace Core.DailyReward
{
    public class DailyRewardViewHandler : MonoBehaviour
    {
        [SerializeField] DBInt RewardClaimed;
        [SerializeField] SOEvents UpDateState;
        [SerializeField] GameObject ClaimBtnsObj, TimerTextObj;
        [SerializeField] RectTransform[] RewardItems;
        [SerializeField] RectTransform Body, niddleRotator;

        float _durationTweeing = 1f;
        Tween _niddleTween;

        private void OnEnable()
        {
            UpDateState.EventHandler += CheckViewState;
            CheckViewState();
        }

        private void OnDisable()
        {
            UpDateState.EventHandler -= CheckViewState;

            if (_niddleTween != null && _niddleTween.IsActive())
                _niddleTween.Kill();
        }

        private void CheckViewState()
        {
            if (RewardClaimed.Value == 0)
            {
                ClaimBtnsObj.SetActive(true);
                TimerTextObj.SetActive(false);
            }
            else
            {
                ClaimBtnsObj.SetActive(false);
                TimerTextObj.SetActive(true);

                _niddleTween = niddleRotator.DORotate(new Vector3(0, 0, -360), _durationTweeing, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);

            }
            Body.DOAnchorPosX(0, _durationTweeing).SetEase(Ease.OutBack).OnComplete(() =>
            {
                for (int i = 0; i < RewardItems.Length; i++)
                {
                    RewardItems[i].DOScale(Vector3.one, _durationTweeing).SetEase(Ease.OutBack);
                }
            });

        }

        public void CloseDailyPanel()
        {
            Body.DOAnchorPosX(1500, _durationTweeing).SetEase(Ease.InBack).OnComplete(() => gameObject.SetActive(false));
        }
    }
}
