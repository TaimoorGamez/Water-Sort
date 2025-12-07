using UnityEngine;

namespace Core.Plugins
{
    public class Initialization : ScriptableObject
    {
        public virtual void InitPlugin()
        {

        }
    }

    public class AdHandler : ScriptableObject
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

    [System.Serializable]
    public class AdConfig
    {
        public bool CanShowAds, CanPurchase;
        public float Ad_Show_Time;
        public bool Interstitial, Rewarded;
    }
}
