using UnityEngine;
using Core.DB.Variables;
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;

namespace Core.Plugins.Ads
{
    public class AdsManager : MonoBehaviour
    {
        [SerializeField] AdDataHandler AdData;
        [SerializeField] AdHandler RewardedAd, IntertitialAd;

        public bool IsInitialized = false, AdTimerComplete = false, AdPlaying = false, CanAddMoves = false, CanMultiply = false, CanDoubleReward = false,
                    CanSpin = false, CanCap = false, CanSpray = false, CanBlockAds = false, CanFlame = false, CanUndo = false,
                                    CanAddExtraTube = false, CanSwitchColor = false;

        public static AdsManager I { get; private set; }

        private void Start()
        {
            if (I == null)
            {
                I = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        public void InitPlugin()
        {
            #if UNITY_EDITOR
                InitAds();  
            #else
                RequestConsentInfo();
            #endif
        }

        void RequestConsentInfo()
        {
            if(!AdData.AdData.CanShowAds)
                return;

            ConsentRequestParameters request = new ConsentRequestParameters
            {
                TagForUnderAgeOfConsent = false
            };

            ConsentInformation.Update(request, (formError) =>
            {
                if (formError != null)
                {
                    Debug.Log("ConsentInfo error: " + formError.Message);
                    return;
                }

                if (ConsentInformation.IsConsentFormAvailable())
                {
                    LoadConsentForm();
                }
                else
                {
                    InitAds();
                }
            });
        }

        void LoadConsentForm()
        {
            ConsentForm.Load((form, loadError) =>
            {
                if (loadError != null)
                {
                    Debug.Log("ConsentForm load error: " + loadError.Message);
                    return;
                }

                form.Show((formError) =>
                {
                    if (formError != null)
                    {
                        Debug.Log("ConsentForm show error: " + formError.Message);
                    }

                    if (ConsentInformation.CanRequestAds())
                    {
                        InitAds();
                    }
                });
            });
        }

        void InitAds()
        {
            if (!AdData.AdData.CanShowAds)
                return;
            
            try
            {
                MobileAds.Initialize((InitializationStatus initstatus) =>
                {
                    if (initstatus == null)
                    {
                        Debug.LogError("InitializationStatus is null!");
                        return;
                    }
                    MobileAds.RaiseAdEventsOnUnityMainThread = true;
                    IsInitialized = true;
                    if (AdData.AdData.Rewarded)
                    {
                        RewardedAd.LoadAd();
                    }

                    if (AdData.AdData.Interstitial && DBVariablesHolder.NoAds.Value == 0)
                    {
                        IntertitialAd.LoadAd();
                    }
                });
            }
            catch (System.Exception ex)
            {
                Debug.LogError("AdMob Initialization crashed: " + ex.Message);
            }
        }
    }
}
