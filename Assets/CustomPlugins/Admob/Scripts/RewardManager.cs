using UnityEngine;
using Core.Events;
using Core.DB.Variables;
using System.Collections;

namespace Core.Plugins.Ads
{
    public class RewardManager : MonoBehaviour
    {
        [SerializeField] AdDataHandler AdData;
        [SerializeField] AdHandler IntertitialAd;

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
                if (AdsManager.I.CanAddMoves)
                {
                    AdsManager.I.CanAddMoves = false;
                    SimpleEventsHolder.AddMovesEvent?.Invoke();
                    _isEnable = false;
                }
                else if (AdsManager.I.CanUndo)
                {
                    AdsManager.I.CanUndo = false;
                    SimpleEventsHolder.RewardUndoEvent?.Invoke();
                    _isEnable = false;
                }
                else if (AdsManager.I.CanAddExtraTube)
                {
                    AdsManager.I.CanAddExtraTube = false;
                    SimpleEventsHolder.RewardExtraTubeEvent?.Invoke();
                    _isEnable = false;
                }
                else if (AdsManager.I.CanSwitchColor)
                {
                    AdsManager.I.CanSwitchColor = false;
                    SimpleEventsHolder.RewardSwapColor?.Invoke();
                    _isEnable = false;
                }
                else if (AdsManager.I.CanMultiply)
                {
                    AdsManager.I.CanMultiply = false;
                    SimpleEventsHolder.MultiplayRewardEvent?.Invoke();
                    _isEnable = false;
                }
                else if (AdsManager.I.CanDoubleReward)
                {
                    AdsManager.I.CanDoubleReward = false;
                    SimpleEventsHolder.DoubleDailyRewardEvent?.Invoke();
                    _isEnable = false;
                }
                else if (AdsManager.I.CanSpin)
                {
                    AdsManager.I.CanSpin = false;
                    SimpleEventsHolder.RewardSpinWheelEvent?.Invoke();
                    _isEnable = false;
                }
                else if (AdsManager.I.CanCap)
                {
                    AdsManager.I.CanCap = false;
                    SimpleEventsHolder.BuyCaps?.Invoke();
                    _isEnable = false;
                }
                else if (AdsManager.I.CanSpray)
                {
                    AdsManager.I.CanSpray = false;
                    SimpleEventsHolder.BuySprays?.Invoke();
                    _isEnable = false;
                }
                else if (AdsManager.I.CanFlame)
                {
                    AdsManager.I.CanFlame = false;
                    SimpleEventsHolder.BuyFlames?.Invoke();
                    _isEnable = false;
                }
                else if (AdsManager.I.CanBlockAds)
                {
                    AdsManager.I.CanBlockAds = false;
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
            if (DBVariablesHolder.NoAds.Value != 1)
            {
                _adsRotine = StartCoroutine(LoadAds());
            }
        }

        IEnumerator LoadAds()
        {
            yield return new WaitForSeconds(AdData.AdData.Ad_Show_Time);
            AdsManager.I.AdTimerComplete = true;

            if (AdData.AdData.Interstitial)
                IntertitialAd.LoadAd();
        }

        void StopAds()
        {
            if (_adsRotine != null)
            {
                StopCoroutine(_adsRotine);
            }
            AdsManager.I.AdTimerComplete = false;
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
        }
    }
}
