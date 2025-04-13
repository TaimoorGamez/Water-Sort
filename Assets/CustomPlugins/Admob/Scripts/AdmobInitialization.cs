using UnityEngine;
using Core.Events;
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

        public override void InitPlugin()
        {
            #if UNITY_EDITOR
                Debug.Log("Running in Unity Editor. Skipping consent and initializing ads.");
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
                    Debug.LogWarning("ConsentInfo error: " + formError.Message);
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
                    Debug.LogWarning("ConsentForm load error: " + loadError.Message);
                    return;
                }

                form.Show((formError) =>
                {
                    if (formError != null)
                    {
                        Debug.LogWarning("ConsentForm show error: " + formError.Message);
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
                Debug.Log("line 82");
                MobileAds.Initialize((InitializationStatus initstatus) =>
                {
                    Debug.Log("line 85");
                    if (initstatus == null)
                    {
                        Debug.LogError("InitializationStatus is null!");
                        return;
                    }

                    MobileAds.RaiseAdEventsOnUnityMainThread = true;

                    if (AdData.AdData.Rewarded)
                    {
                        Debug.Log("Loading rewarded ad");
                        RewardedAd.LoadAd();
                    }

                    if (AdData.AdData.Interstitial && NoAds.Value == 0)
                    {
                        Debug.Log("Invoking start ad loading event");
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
