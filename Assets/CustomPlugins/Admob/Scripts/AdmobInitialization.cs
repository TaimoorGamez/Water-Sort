using UnityEngine;
using Core.Variables;
using Core.DB.Variables;
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
using Core.Events;

namespace Core.Plugins.Ads
{
    public class AdmobInitialization : MonoBehaviour
    {
        [SerializeField] AdDataHandler AdData;
        [SerializeField] DBInt NoAds;
        [SerializeField] AdHandler RewardedAd;
        [SerializeField] SOInterger AdmobInitialized;

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
                    //Debug.Log("line 85");
                    if (initstatus == null)
                    {
                        Debug.LogError("InitializationStatus is null!");
                        return;
                    }
                    MobileAds.RaiseAdEventsOnUnityMainThread = true;
                    AdmobInitialized.Value = 1;
                    if (AdData.AdData.Rewarded)
                    {
                        //Debug.Log("Loading rewarded ad");
                        RewardedAd.LoadAd();
                    }

                    if (AdData.AdData.Interstitial && NoAds.Value == 0)
                    {
                        SimpleEventsHolder.StartAdLoaing?.Invoke();
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
