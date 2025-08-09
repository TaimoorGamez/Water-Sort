using UnityEngine;
using Core.Events;
using Core.DB.Variables;

namespace ProjectCore.DailyReward
{
    public class DailyRewardViewHandler : MonoBehaviour
    {
        [SerializeField] DBInt RewardClaimed;
        [SerializeField] SOEvents UpDateState;
        [SerializeField] GameObject ClaimBtnsObj, TimerTextObj, RewardsObj;

        private void Awake()
        {
            CheckViewState();
        }

        private void OnEnable()
        {
            UpDateState.EventHandler += CheckViewState;
        }

        private void CheckViewState()
        {
            RewardsObj.SetActive(false);
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
            RewardsObj.SetActive(true);
        }
        private void OnDisable()
        {
            UpDateState.EventHandler -= CheckViewState;
        }
    }
}
