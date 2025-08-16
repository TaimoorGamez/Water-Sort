using DG.Tweening;
using UnityEngine;

namespace Core.SpinWheel
{
    public class SpinWheelView : MonoBehaviour, ISpinWheel
    {
        [SerializeField] Transform SegmentParent;
        [SerializeField] SpinWheelSegment SegmentPrefab;
        [SerializeField] SpinWheelData SpinWheelData;

        float _segmentAngle, _spinDuration = 5f;
        bool _onceClicked = false;

        void Start()
        {
            CreateWheelView();
        }

        void CreateWheelView()
        {
            int rewardCount = SpinWheelData.SpinWheelRewards.Length;
            _segmentAngle = 360f / rewardCount;
            float fillAmount = 1f / rewardCount;
            for (int i = 0; i < rewardCount; i++)
            {
                SpinWheelConfige reward = SpinWheelData.SpinWheelRewards[i];

                SpinWheelSegment segment = Instantiate(SegmentPrefab, SegmentParent);
                segment.transform.localRotation = Quaternion.Euler(0, 0, -i * _segmentAngle);

                segment.Initialize(reward.Icon, reward.Amount, reward.SegmentGradient, fillAmount);
            }
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
            });
        }

    }
}
