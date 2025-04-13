using UnityEngine;
using Core.Events;
using Core.Variables;
using GoogleMobileAds.Api;

namespace Core.Plugins.Ads
{
    [CreateAssetMenu(fileName = "Rewarded", menuName = "ScriptableObjects/Plugin/Admob/Rewarded")]
    public class SORewardedAd : AdHandler
    {
        [SerializeField] SOEvents GrantRewardEvent;
        [SerializeField] SOInterger CanSpin, CanDoubleDailyReward, CanRewardUndo, CanAddMoves, AdPlaying;
        [SerializeField] SOIntegerEvents ShowToastEvent;

        RewardedAd _rewardedAd;
        string _adUnitId;

        public override void LoadAd()
        {
            if (IsTestAd)
            {
                _adUnitId = TestId;
            }
            else
            {
                _adUnitId = AdId;
            }

            if (_rewardedAd != null)
            {
                _rewardedAd.Destroy();
                _rewardedAd = null;
            }

            var adRequest = new AdRequest();
            RewardedAd.Load(_adUnitId, adRequest,
                (RewardedAd ad, LoadAdError error) =>
                {
                    if (error != null || ad == null)
                    {
                        return;
                    }
                    _rewardedAd = ad;
                    RegisterEventHandlers(ad);
                });
        }

        public override bool IsAdAvailable
        {
            get
            {
                return _rewardedAd != null && _rewardedAd.CanShowAd() && AdPlaying.Value == 0; 
            }
        }

        public override void ShowAd(string detail = "")
        {
            if (IsAdAvailable)
            {
                AdPlaying.Value = 1;
                _rewardedAd.Show((Reward reward) =>
                {
                    GrantReward(detail);
                });
            }
            else
            {
                ShowToastEvent.InvokeSOEvent(0);
                LoadAd();
            }
        }

        private void RegisterEventHandlers(RewardedAd ad)
        {
            // Raised when the ad is estimated to have earned money.
            ad.OnAdPaid += (AdValue adValue) =>
            {
                //Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                //    adValue.Value,
                //    adValue.CurrencyCode));
            };
            // Raised when an impression is recorded for an ad.
            ad.OnAdImpressionRecorded += () =>
            {
                //Debug.Log("Rewarded ad recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            ad.OnAdClicked += () =>
            {
                //Debug.Log("Rewarded ad was clicked.");
            };
            // Raised when an ad opened full screen content.
            ad.OnAdFullScreenContentOpened += () =>
            {
                //Debug.Log("Rewarded ad full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            ad.OnAdFullScreenContentClosed += () =>
            {
                //Debug.Log("Rewarded ad full screen content closed.");
                AdPlaying.Value = 0;
                LoadAd();
            };
            // Raised when the ad failed to open full screen content.
            ad.OnAdFullScreenContentFailed += (AdError error) =>
            {
                //Debug.LogError("Rewarded ad failed to open full screen content " +
                //               "with error : " + error);
                AdPlaying.Value = 0;
                LoadAd();
            };
        }

        void GrantReward(string rewardName)
        {
            switch (rewardName)
            {
                case "MultiplyCoins":
                    CanSpin.Value = 1;
                    break;

                case "DoubleDailyReward":
                    CanDoubleDailyReward.Value = 1;
                    break;

                case "RewardUndo":
                    CanRewardUndo.Value = 1;
                    break;

                case "AddMoves":
                    CanAddMoves.Value = 1;
                    break;
            }
            GrantRewardEvent.InvokeSOEvent();
        }
    }
}
