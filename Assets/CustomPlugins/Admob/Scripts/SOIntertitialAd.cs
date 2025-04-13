using UnityEngine;
using Core.Events;
using Core.Variables;
using Core.DB.Variables;
using GoogleMobileAds.Api;

namespace Core.Plugins.Ads
{
    [CreateAssetMenu(fileName = "Intertitial", menuName = "ScriptableObjects/Plugin/Admob/Intertitial")]
    public class SOIntertitialAd : AdHandler
    {
        [SerializeField] SOEvents StartAdLoaing;
        [SerializeField] SOInterger AdTimerComplete;
        [SerializeField] DBInt NoAdsDB;

        InterstitialAd _interstitialAd;
        string _adUnitId;

        public override void LoadAd()
        {
            if (NoAdsDB.Value != 1)
            {
                if (IsTestAd)
                {
                    _adUnitId = TestId;
                }
                else
                {
                    _adUnitId = AdId;
                }

                if (_interstitialAd != null)
                {
                    _interstitialAd.Destroy();
                    _interstitialAd = null;
                }

                var adRequest = new AdRequest();

                InterstitialAd.Load(_adUnitId, adRequest,
                    (InterstitialAd ad, LoadAdError error) =>
                    {
                        if (error != null || ad == null)
                        {
                            return;
                        }
                        _interstitialAd = ad;
                        RegisterEventHandlers(ad);
                    });
            }
        }

        public override bool IsAdAvailable
        {
            get
            {
                return _interstitialAd != null && _interstitialAd.CanShowAd() && AdTimerComplete.Value == 1;
            }
        }

        public override void ShowAd(string detail = "")
        {
            if (IsAdAvailable)
            {
                _interstitialAd.Show();
            }
            else
            {
                AdTimerComplete.Value = 0;
                StartAdLoaing.InvokeSOEvent();
            }
        }

        private void RegisterEventHandlers(InterstitialAd interstitialAd)
        {
            // Raised when the ad is estimated to have earned money.
            interstitialAd.OnAdPaid += (AdValue adValue) =>
            {
                //Debug.Log("Interstitial ad paid {0} {1}.",
                //    adValue.Value,
                //    adValue.CurrencyCode);
            };
            // Raised when an impression is recorded for an ad.
            interstitialAd.OnAdImpressionRecorded += () =>
            {
                //Debug.Log("Interstitial ad recorded an impression.");
            };
            // Raised when a click is recorded for an ad.
            interstitialAd.OnAdClicked += () =>
            {
                //Debug.Log("Interstitial ad was clicked.");
            };
            // Raised when an ad opened full screen content.
            interstitialAd.OnAdFullScreenContentOpened += () =>
            {
                //Debug.Log("Interstitial ad full screen content opened.");
            };
            // Raised when the ad closed full screen content.
            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                AdTimerComplete.Value = 0;
                StartAdLoaing.InvokeSOEvent();
            };
            // Raised when the ad failed to open full screen content.
            interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
            {
                AdTimerComplete.Value = 0;
                StartAdLoaing.InvokeSOEvent();
            };
        }
    }
}
