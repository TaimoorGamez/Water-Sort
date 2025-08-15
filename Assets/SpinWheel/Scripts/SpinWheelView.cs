using UnityEngine;
using System.Collections;

namespace Core.SpinWheel
{
    public class SpinWheelView : MonoBehaviour
    {
        [SerializeField] Transform SegmentParent;
        [SerializeField] SpinWheelSegment SegmentPrefab;

        SpinWheelData _spinWheelData;
        string _dataPath = "SpinWheel/SpinWheelData";
        Coroutine _loadingRotine;

        private void OnEnable()
        {
            StopLoadRotine();
        }

        private void OnDisable()
        {
            StopLoadRotine();
        }

        void Start()
        {
            _loadingRotine = StartCoroutine(LoadDataAsync());
        }

        IEnumerator LoadDataAsync()
        {
            ResourceRequest request = Resources.LoadAsync<SpinWheelData>(_dataPath);
            yield return request;

            _spinWheelData = request.asset as SpinWheelData;
            if (_spinWheelData == null)
            {
                Debug.LogError($"SpinWheelData.asset not found at Resources/{_dataPath}!");
                yield break;
            }
            else
            {
                CreateWheelView();
            }
        }

        void CreateWheelView()
        {
            StopLoadRotine();
            for (int i = 0; i < SegmentParent.childCount; i++)
            {
                Destroy(SegmentParent.GetChild(i).gameObject);
            }

            int rewardCount = _spinWheelData.SpinWheelRewards.Length;
            float angleStep = 360f / rewardCount;

            for (int i = 0; i < rewardCount; i++)
            {
                SpinWheelConfige reward = _spinWheelData.SpinWheelRewards[i];

                SpinWheelSegment segment = Instantiate(SegmentPrefab, SegmentParent);
                segment.transform.localRotation = Quaternion.Euler(0, 0, -i * angleStep);

                segment.Initialize(reward.Icon, reward.Amount, reward.SegmentGradient);
            }
        }

        void StopLoadRotine()
        {
            if (_loadingRotine != null)
            {
                StopCoroutine(_loadingRotine);
            }
        }
    }
}
