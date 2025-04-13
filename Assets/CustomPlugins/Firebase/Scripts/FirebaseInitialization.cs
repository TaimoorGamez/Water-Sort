using Firebase;
using UnityEngine;
using Core.Variables;
using Core.Plugins.Ads;
using Firebase.RemoteConfig;

namespace Core.Plugins.Firebase
{
    [CreateAssetMenu(fileName = "FirebaseInit", menuName = "ScriptableObjects/Plugin/Firebase/Init")]
    public class FirebaseInitialization : Initialization
    {
        [SerializeField] SOInterger IsFirebaseInit;
        [SerializeField] AdDataHandler AdData;
        [SerializeField] Initialization AdmobInitilization;

        public override void InitPlugin()
        {
            IsFirebaseInit.Value = 0;
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
            {
                DependencyStatus dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    FetchRemoteConfig();
                    IsFirebaseInit.Value = 1;
                }
                else
                {
                    Debug.Log("Could not resolve all Firebase dependencies:" + dependencyStatus);
                }
            });
        }

        void FetchRemoteConfig()
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                Debug.LogError($"Do not use Firebase until it is properly initialized by calling");
                return;
            }

            Debug.Log("Fetching data...");
            FirebaseRemoteConfig remoteConfig = FirebaseRemoteConfig.DefaultInstance;
            remoteConfig.FetchAsync(System.TimeSpan.Zero).ContinueWith(
               Task =>
               {
                   if (!Task.IsCompleted)
                   {
                       Debug.LogError($"{nameof(remoteConfig.FetchAsync)} incomplete: Status '{Task.Status}'");
                       return;
                   }
                   ActivateRetrievedRemoteConfigValues();
               });
        }

        private void ActivateRetrievedRemoteConfigValues()
        {
            FirebaseRemoteConfig remoteConfig = FirebaseRemoteConfig.DefaultInstance;
            ConfigInfo info = remoteConfig.Info;
            if (info.LastFetchStatus == LastFetchStatus.Success)
            {
                remoteConfig.ActivateAsync().ContinueWith(
                   Task =>
                   {
                       string adJson = remoteConfig.GetValue("AdConfig").StringValue;
                       AdData.AdData = JsonUtility.FromJson<AdConfig>(adJson);
                       AdmobInitilization.InitPlugin();
                   });
            }
        }
    }
}
