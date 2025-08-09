using DG.Tweening;
using UnityEngine;
using Core.Events;
using Core.DB.Variables;

namespace ProjectCore.DailyReward
{
    public class DailyRewardViewHandler : MonoBehaviour
    {
        [SerializeField] DBInt RewardClaimed;
        [SerializeField] SOEvents UpDateState;
        [SerializeField] GameObject ClaimBtnsObj, TimerTextObj;
        [SerializeField] RectTransform[] RewardItems;
        [SerializeField] RectTransform Body;

        float _durationTweeing = 1f;

        private void Start()
        {
            if(RewardClaimed.Value == 1)
            {
                UpDateState.InvokeSOEvent();
            }
            CheckViewState();
        }

        private void OnEnable()
        {
            UpDateState.EventHandler += CheckViewState;
        }

        private void OnDisable()
        {
            UpDateState.EventHandler -= CheckViewState;
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
            }
            Body.DOAnchorPosX(0, _durationTweeing).SetEase(Ease.OutBack).OnComplete(() =>
            {
                for (int i = 0; i < RewardItems.Length; i++)
                {
                    RewardItems[i].DOScale(Vector3.one, _durationTweeing).SetEase(Ease.OutBack);
                }
            });

        }
    }
}
