using DG.Tweening;
using UnityEngine;
using Core.Events;
using Core.DB.Variables;

namespace Core.Screen
{
    public class DailyRewardScreen : UiScreens
    {
        [SerializeField] DBInt RewardClaimed;
        [SerializeField] SOEvents UpDateState;
        [SerializeField] SOIntegerEvents SoundEffectEvent;
        [SerializeField] GameObject ClaimBtnsObj, TimerTextObj;
        [SerializeField] RectTransform[] RewardItems;
        [SerializeField] RectTransform NiddleRotator;

        float _niddleTweeingTime = 1f;
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

                _niddleTween = NiddleRotator.DORotate(new Vector3(0, 0, -360), _niddleTweeingTime, RotateMode.FastBeyond360).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);

            }
            OnOpen();
        }

        public override void OnOpen()
        {
            SoundEffectEvent.InvokeSOEvent(3);
            Body.DOAnchorPosX(0, _transitionDuration).SetEase(Ease.OutBack).OnComplete(() =>
            {
                for (int i = 0; i < RewardItems.Length; i++)
                {
                    RewardItems[i].DOScale(Vector3.one, _transitionDuration).SetEase(Ease.OutBack);
                }
            });
        }

        public override void OnClose()
        {
            SoundEffectEvent.InvokeSOEvent(2);
            Body.DOAnchorPosX(1500, _transitionDuration / 2).SetEase(Ease.InBack).OnComplete(() => gameObject.SetActive(false));
        }
    }
}
