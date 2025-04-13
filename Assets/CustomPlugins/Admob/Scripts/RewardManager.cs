using UnityEngine;
using Core.Events;
using Core.Variables;
using Core.DB.Variables;
using System.Collections;

namespace Core.Plugins.Ads
{
    public class RewardManager : MonoBehaviour
    {
        [SerializeField] DBInt NoAds;
        [SerializeField] AdDataHandler AdData;
        [SerializeField] AdHandler IntertitialAd;
        [SerializeField] SOEvents NoAdsBuyEvent, StartLoadingAdsEvent, GiveRewardEvent, CompletePanelRewardEvent, DoubleDailyRewardEvent;
        [SerializeField] SOInterger AdTimerComplete, CanRewardSpin, CanRewardOnComplete;

        Coroutine _rewardRotine, _adsRotine;
        bool _isEnable = false;

        private void OnEnable()
        {
            //GiveRewardEvent.EventHandler += PlayRewardCorotine;
            StartLoadingAdsEvent.EventHandler += StartLoadingAds;
            NoAdsBuyEvent.EventHandler += StopAds;
        }

        private void OnDisable()
        {
            //GiveRewardEvent.EventHandler -= PlayRewardCorotine;
            StartLoadingAdsEvent.EventHandler -= StartLoadingAds;
            NoAdsBuyEvent.EventHandler -= StopAds;
            CustomDisable();
        }

        void PlayRewardCorotine()
        {
            _isEnable = true;
            //_rewardRotine = StartCoroutine(RewardCorotine());
        }

        //IEnumerator RewardCorotine()
        //{
        //    WaitForSeconds wait = new WaitForSeconds(0.01f);
        //    while (_isEnable)
        //    {
        //        yield return wait;
        //        if (CanRewardGems.GetValue())
        //        {
        //            CanRewardGems.SetValue(false);
        //            RewardExtraGems.Invoke();
        //            _isEnable = false;
        //        }
        //        else if (CanRewardCoins.GetValue())
        //        {
        //            CanRewardCoins.SetValue(false);
        //            RewardExtraCoinsEvent.Invoke();
        //            _isEnable = false;
        //        }
        //        else if (CanRewardSpin.GetValue())
        //        {
        //            CanRewardSpin.SetValue(false);
        //            RewardSpinWheelEvent.Invoke();
        //            _isEnable = false;
        //        }
        //        else if (CanRewardOnComplete.GetValue())
        //        {
        //            CanRewardOnComplete.SetValue(false);
        //            CompletePanelRewardEvent.Invoke();
        //            _isEnable = false;
        //        }
        //        else if (CanReviveReward.GetValue())
        //        {
        //            CanReviveReward.SetValue(false);
        //            RevivePanelRewardEvent.Invoke();
        //            _isEnable = false;
        //        }
        //        else if (CanRewardRoll.GetValue())
        //        {
        //            CanRewardRoll.SetValue(false);
        //            RewardFreeRollEvent.Invoke();
        //            _isEnable = false;
        //        }
        //        else if (CanRewardStars.GetValue())
        //        {
        //            CanRewardStars.SetValue(false);
        //            RewardExtraStars.Invoke();
        //            _isEnable = false;
        //        }
        //    }
        //    CustomDisable();
        //}

        void StartLoadingAds()
        {
            Debug.Log("here");
            if (NoAds.Value != 1)
            {
                _adsRotine = StartCoroutine(LoadAds());
            }
        }

        IEnumerator LoadAds()
        {
            yield return new WaitForSeconds(AdData.AdData.Ad_Show_Time);
            AdTimerComplete.Value = 1;

            if (AdData.AdData.Interstitial)
                IntertitialAd.LoadAd();
        }

        void StopAds()
        {
            if (_adsRotine != null)
            {
                StopCoroutine(_adsRotine);
            }
            AdTimerComplete.Value = 0;
        }

        void CustomDisable()
        {
            _isEnable = false;
            if (_rewardRotine != null)
            {
                StopCoroutine(_rewardRotine);
            }
            if (_adsRotine != null)
            {
                StopCoroutine(_adsRotine);
            }
            AdTimerComplete.Value = 0;
        }
    }
}
