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
        [SerializeField] SOInterger AdTimerComplete, CanAddMoves, CanMultiply, CanDoubleReward, CanSpin, CanCaps, CanSprays, CanBlockAds, CanFlames, CanUndo,
                                    CanAddExtraTube, CanSwitchColor;

        Coroutine _rewardRotine, _adsRotine;
        bool _isEnable = false;

        private void OnEnable()
        {
            SimpleEventsHolder.GrantRewardEvent += PlayRewardCorotine;
            SimpleEventsHolder.StartLoadingAdsEvent += StartLoadingAds;
            SimpleEventsHolder.NoAdsBuyEvent += StopAds;
        }

        private void OnDisable()
        {
            SimpleEventsHolder.GrantRewardEvent -= PlayRewardCorotine;
            SimpleEventsHolder.StartLoadingAdsEvent -= StartLoadingAds;
            SimpleEventsHolder.NoAdsBuyEvent -= StopAds;
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
                    SimpleEventsHolder.AddMovesEvent?.Invoke();
                    _isEnable = false;
                }
                else if (CanUndo.Value == 1)
                {
                    CanUndo.Value = 0;
                    SimpleEventsHolder.RewardUndoEvent?.Invoke();
                    _isEnable = false;
                }
                else if (CanAddExtraTube.Value == 1)
                {
                    CanAddExtraTube.Value = 0;
                    SimpleEventsHolder.RewardExtraTubeEvent?.Invoke();
                    _isEnable = false;
                }
                else if (CanSwitchColor.Value == 1)
                {
                    CanSwitchColor.Value = 0;
                    SimpleEventsHolder.RewardSwapColor?.Invoke();
                    _isEnable = false;
                }
                else if (CanMultiply.Value == 1)
                {
                    CanMultiply.Value = 0;
                    SimpleEventsHolder.MultiplayRewardEvent?.Invoke();
                    _isEnable = false;
                }
                else if (CanDoubleReward.Value == 1)
                {
                    CanDoubleReward.Value = 0;
                    SimpleEventsHolder.DoubleDailyRewardEvent?.Invoke();
                    _isEnable = false;
                }
                else if (CanSpin.Value == 1)
                {
                    CanSpin.Value = 0;
                    SimpleEventsHolder.RewardSpinWheelEvent?.Invoke();
                    _isEnable = false;
                }
                else if (CanCaps.Value == 1)
                {
                    CanCaps.Value = 0;
                    SimpleEventsHolder.BuyCaps?.Invoke();
                    _isEnable = false;
                }
                else if (CanSprays.Value == 1)
                {
                    CanSprays.Value = 0;
                    SimpleEventsHolder.BuySprays?.Invoke();
                    _isEnable = false;
                }
                else if (CanFlames.Value == 1)
                {
                    CanFlames.Value = 0;
                    SimpleEventsHolder.BuyFlames?.Invoke();
                    _isEnable = false;
                }
                else if (CanBlockAds.Value == 1)
                {
                    CanBlockAds.Value = 0;
                    SimpleEventsHolder.AdsBlockerEvent?.Invoke();
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
