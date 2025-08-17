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
        [SerializeField] SOEvents NoAdsBuyEvent, StartLoadingAdsEvent, GrantRewardEvent, CompletePanelRewardEvent, AddMovesEvent,
                                  DoubleDailyRewardEvent, SpinEvent;
        [SerializeField] SOInterger AdTimerComplete, CanAddMoves, CanMultiply, CanDoubleReward, CanSpin;

        Coroutine _rewardRotine, _adsRotine;
        bool _isEnable = false;

        private void OnEnable()
        {
            GrantRewardEvent.EventHandler += PlayRewardCorotine;
            StartLoadingAdsEvent.EventHandler += StartLoadingAds;
            NoAdsBuyEvent.EventHandler += StopAds;
        }

        private void OnDisable()
        {
            GrantRewardEvent.EventHandler -= PlayRewardCorotine;
            StartLoadingAdsEvent.EventHandler -= StartLoadingAds;
            NoAdsBuyEvent.EventHandler -= StopAds;
            CustomDisable();
        }

        void PlayRewardCorotine()
        {
            _isEnable = true;
            _rewardRotine = StartCoroutine(RewardCorotine());
        }

        IEnumerator RewardCorotine()
        {
            WaitForSeconds wait = new WaitForSeconds(0.01f);
            while (_isEnable)
            {
                yield return wait;
                if (CanAddMoves.Value == 1)
                {
                    CanAddMoves.Value = 0;
                    AddMovesEvent.InvokeSOEvent();
                    _isEnable = false;
                }
                else if (CanMultiply.Value == 1)
                {
                    CanMultiply.Value = 0;
                    CompletePanelRewardEvent.InvokeSOEvent();
                    _isEnable = false;
                }
                else if (CanDoubleReward.Value == 1)
                {
                    CanDoubleReward.Value = 0;
                    DoubleDailyRewardEvent.InvokeSOEvent();
                    _isEnable = false;
                }
                else if (CanSpin.Value == 1)
                {
                    CanSpin.Value = 0;
                    SpinEvent.InvokeSOEvent();
                    _isEnable = false;
                }
            }
            if (_rewardRotine != null)
            {
                StopCoroutine(_rewardRotine);
            }
        }

        void StartLoadingAds()
        {
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
