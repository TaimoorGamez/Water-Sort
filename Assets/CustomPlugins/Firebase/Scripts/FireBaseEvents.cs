using UnityEngine;
using Core.Variables;
using Core.DB.Variables;
using Firebase.Analytics;

namespace Core.Plugins.Firebase
{
    [CreateAssetMenu(fileName = "FbEvent", menuName = "ScriptableObjects/Plugin/Firebase/Events")]
    public class FireBaseEvents : ScriptableObject
    {
        [SerializeField] DBInt CashWallet;
        [SerializeField] SOInterger IsFirebaseInit;

        public void LevelStartEvent(string lvlNum)
        {
            if (IsFirebaseInit.Value == 1)
            {
                Parameter[] parameters = {
            new Parameter ("level", lvlNum),
            new Parameter ("current_gold", CashWallet.Value),
                 };
                FirebaseAnalytics.LogEvent("level_start", parameters);
            }
        }

        public void LevelCompleteEvent(int lvlNum, double lvlTime)
        {
            if (IsFirebaseInit.Value == 1)
            {
                Parameter[] parameters = {
            new Parameter ("level", lvlNum.ToString()),
            new Parameter ("timeplayed", lvlTime.ToString()),
            };
                FirebaseAnalytics.LogEvent("level_complete", parameters);
            }
        }

        public void LevelFailEvent(int lvlNum, string failNum, double lvlTime)
        {
            if (IsFirebaseInit.Value == 1)
            {
                Parameter[] parameters = {
            new Parameter ("level", lvlNum.ToString()),
            new Parameter ("failcount", failNum),
             };
                FirebaseAnalytics.LogEvent("level_fail", parameters);
            }
        }

        public void EarnCoinsEvent(string coinsType, long amount, string sourceOfIncome)
        {
            if (IsFirebaseInit.Value == 1)
            {
                Parameter[] parameters = {
            new Parameter ("virtual_currency_name", coinsType),
            new Parameter ("value", amount),
            new Parameter ("source", sourceOfIncome),
            };
                FirebaseAnalytics.LogEvent("earn_virtual_currency", parameters);
            }
        }

        public void SpendCoinsEvent(string coinsType, long amount, string purchaseName)
        {
            if (IsFirebaseInit.Value == 1)
            {
                Parameter[] parameters = {
            new Parameter ("virtual_currency_name", coinsType),
            new Parameter ("value", amount),
            new Parameter ("item_name", purchaseName),
             };
                FirebaseAnalytics.LogEvent("spend_virtual_currency", parameters);
            }
        }

        public void LogEvent(string eventString)
        {
            if (IsFirebaseInit.Value == 1)
            {
                FirebaseAnalytics.LogEvent(eventString);
            }
        }

        //public void AdRevenue(MaxSdkBase.AdInfo adInfo)
        //{
        //    var impressionParameters = new[]
        //    {
        //    new Parameter("ad_platform", "AppLovin"),
        //    new Parameter("ad_source", adInfo.NetworkName),
        //    new Parameter("ad_unit_name", adInfo.AdUnitIdentifier),
        //    new Parameter("ad_format", adInfo.AdFormat),
        //    new Parameter("value", adInfo.Revenue),
        //    new Parameter("currency", "USD"), // All AppLovin revenue is sent in USD
        //};

        //    FirebaseAnalytics.LogEvent("ad_impression", impressionParameters);
        //}
    }
}
