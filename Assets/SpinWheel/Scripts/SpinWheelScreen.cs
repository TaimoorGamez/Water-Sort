using TMPro;
using DG.Tweening;
using Core.Events;
using UnityEngine;
using UnityEngine.UI;
using Core.SpinWheel;
using Core.DB.Variables;

namespace Core.Screen
{
    public class SpinWheelScreen : UiScreens, ISpinWheel
    {
        [SerializeField] GameObject SpinBtn, RvBtn, SpinNotification;
        [SerializeField] RectTransform SegmentParent, Shine, RewardPanel, Wheel;
        [SerializeField] SpinWheelSegment SegmentPrefab;
        [SerializeField] SpinWheelData SpinWheelData;
        [SerializeField] Color RewardedColor;
        [SerializeField] Image RewardIcon;
        [SerializeField] TextMeshProUGUI RewardAmount;

        float _segmentAngle, _spinDuration = 5f, _tweenDiration = 0.5f;
        bool _onceClicked = false;
        SpinWheelSegment[] _allSpinSegments;

        private void OnEnable()
        {
            SimpleEventsHolder.RewardSpinWheelEvent += RewardedSpin;
            if (DBIntsHolder.SpinAvailable.Value == 1)
            {
                SpinBtn.SetActive(true);
                RvBtn.SetActive(false);
                _onceClicked = false;
                SpinNotification.SetActive(true);
            }
            else
            {
                RvBtn.SetActive(true);
                SpinBtn.SetActive(false);
                SpinNotification.SetActive(false);
            }
            OnOpen();
        }

        private void OnDisable()
        {
            SimpleEventsHolder.RewardSpinWheelEvent -= RewardedSpin;
        }

        void Start()
        {
            CreateWheelView();
        }

        void CreateWheelView()
        {
            int rewardCount = SpinWheelData.SpinWheelRewards.Length;
            _allSpinSegments = new SpinWheelSegment[rewardCount];
            _segmentAngle = 360f / rewardCount;
            float fillAmount = 1f / rewardCount;
            for (int i = 0; i < rewardCount; i++)
            {
                SpinWheelConfige reward = SpinWheelData.SpinWheelRewards[i];
                SpinWheelSegment segment = Instantiate(SegmentPrefab, SegmentParent);
                segment.transform.localRotation = Quaternion.Euler(0, 0, -i * _segmentAngle);
                segment.Initialize(reward.Icon, reward.Amount, reward.SegmentColor, fillAmount);
                _allSpinSegments[i] = segment;
            }
        }

        void RewardedSpin()
        {
            _onceClicked = false;
            Spin();
        }

        public void Spin()
        {
            if (!_onceClicked)
            {
                _onceClicked = true;
                int rewardIndex = GetWeightedRandomIndex();
                StopAtReward(rewardIndex);
            }
        }

        int GetWeightedRandomIndex()
        {
            float totalWeight = 0f;
            int rewardCount = SpinWheelData.SpinWheelRewards.Length;

            for (int i = 0; i < rewardCount; i++)
            {
                totalWeight += SpinWheelData.SpinWheelRewards[i].Weight;
            }
            float randomValue = Random.Range(0, totalWeight);
            float cumulative = 0f;
            for (int i = 0; i < rewardCount; i++)
            {
                cumulative += SpinWheelData.SpinWheelRewards[i].Weight;
                if (randomValue <= cumulative)
                    return i;
            }

            return 0;
        }

        void StopAtReward(int rewardIndex)
        {
            float angleDiff = _segmentAngle / 2;
            float targetAngle = rewardIndex * _segmentAngle;
            targetAngle += angleDiff;
            float finalAngle = (360f * 5) + targetAngle;
            SegmentParent.DOLocalRotate(new Vector3(0, 0, finalAngle), _spinDuration, RotateMode.FastBeyond360).SetEase(Ease.OutQuart).OnComplete(() =>
            {
                SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(3);
                Shine.DOScale(Vector3.one,_tweenDiration).SetEase(Ease.OutBack).OnComplete(()=>Shine.localScale = Vector3.zero).OnComplete(()=>{
                    Shine.localScale = Vector3.zero;
                    RewardIcon.sprite = SpinWheelData.SpinWheelRewards[rewardIndex].Icon;
                    RewardAmount.text = "+"+SpinWheelData.SpinWheelRewards[rewardIndex].Amount.ToString();
                    RewardPanel.DOScale(Vector3.one, _tweenDiration).SetEase(Ease.OutBack).OnComplete(() => {
                        Invoke(nameof(CloseRewardPanel), 1);
                        _allSpinSegments[rewardIndex].ChangeGradient(SpinWheelData.SpinWheelRewards[rewardIndex].SegmentColor);
                    });
                    DBIntsHolder.SpinAvailable.Value = 0;
                    SpinBtn.SetActive(false);
                    SpinNotification.SetActive(false);
                    RvBtn.SetActive(true);
                    SpinWheelData.SpinWheelRewards[rewardIndex].ClaimReward();
                });
                _allSpinSegments[rewardIndex].ChangeGradient(RewardedColor);
            });
            SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(10);
        }

        void CloseRewardPanel()
        {
            SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(2);
            RewardPanel.DOScale(Vector3.zero, _transitionDuration/2).SetEase(Ease.InBack);
        }

        public override void OnOpen()
        {
            SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(3);
            Body.DOAnchorPosX(0, _transitionDuration).SetEase(Ease.OutBack).OnComplete(() => {
                SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(5);
                Wheel.DOScale(Vector3.one, _transitionDuration).SetEase(Ease.OutBack);
            });
        }

        public override void OnClose()
        {
            SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(5);
            Wheel.DOScale(Vector3.zero, _transitionDuration/2).SetEase(Ease.InBack).OnComplete(() => {
                SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(2);
                Body.DOAnchorPosX(-1500, _transitionDuration/2).SetEase(Ease.InBack).OnComplete(() => gameObject.SetActive(false));
            });
        }

    }
}
