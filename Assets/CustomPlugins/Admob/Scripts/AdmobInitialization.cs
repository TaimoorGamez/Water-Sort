using UnityEngine;
using Core.Events;
using Core.Variables;
using Core.DB.Variables;
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;

namespace Core.Plugins.Ads
{
    [CreateAssetMenu(fileName = "AdmobInit", menuName = "ScriptableObjects/Plugin/Admob/Init")]
    public class AdmobInitialization : Initialization
    {
        [SerializeField] AdDataHandler AdData;
        [SerializeField] DBInt NoAds;
        [SerializeField] SOEvents StartAdLoaing;
        [SerializeField] AdHandler RewardedAd;
        [SerializeField] SOInterger AdmobInitialized;

        public override void InitPlugin()
        {
            #if UNITY_EDITOR
                InitAds();  
            #else
                RequestConsentInfo();
            #endif
        }

        void RequestConsentInfo()
        {
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
                        //Debug.Log("Invoking start ad loading event");
                        StartAdLoaing.InvokeSOEvent();
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
