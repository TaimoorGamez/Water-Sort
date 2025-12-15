using UnityEngine;

namespace Core.Plugins
{
    public class AdHandler: MonoBehaviour
    {
        [SerializeField] protected string AdId, TestId;
        [SerializeField] protected bool IsTestAd;

        protected bool _isInit = false;

        private void OnEnable()
        {
            _isInit = false;
        }
        public virtual void LoadAd()
        {

        }

        public virtual void ShowAd(string detail = "")
        {

        }

        public virtual void HideAd()
        {

        }

        public virtual bool IsAdAvailable
        {
            get;
        }
    }

    public class AdConfig
    {
        public bool CanShowAds, CanPurchase;
        public float Ad_Show_Time;
        public bool Interstitial, Rewarded;
    }

    public static class RemoteDataHolder
    {
        public static bool IsInternetWorking = false;
        public static int MaxLevelsAvailable = 0;
        public static AdConfig AdData;
    }
}
