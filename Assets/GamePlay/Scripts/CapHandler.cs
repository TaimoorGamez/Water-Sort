using UnityEngine;
using Core.DB.Variables;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.GamePlay.WaterSort
{
    public class CapHandler : MonoBehaviour
    {
        string _capsPath = "GamePlay/Cap/";
        CapAnimation _myAnimation;
        AsyncOperationHandle _capHandle;

        private void Start()
        {
            LoadCapItem(_capsPath + DBIntsHolder.CurrentActiveCap.Value);
        }

        async void LoadCapItem(string path)
        {
            _capHandle = Addressables.LoadAssetAsync<GameObject>(path);
            await _capHandle.Task;

            if (_capHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Failed to load Addressable prefab at: {path}");
                return;
            }

            _myAnimation = null;
            GameObject obj = Instantiate(_capHandle.Result as GameObject, transform);
            await Task.Yield();
            await Task.Yield();
            _myAnimation = obj.GetComponent<CapAnimation>();

            while (_myAnimation == null)
                await Task.Yield();
        }

        public void PlayCelebration(Color currentColor)
        {
            _myAnimation.gameObject.SetActive(true);
            _myAnimation.PlayCapAnimation(currentColor);
        }

        public void HideCap()
        {
           _myAnimation.gameObject.SetActive(false);
        }

        private async void OnDisable()
        {
            await ReleaseHandler();
        }

        async Task ReleaseHandler()
        {
            if (!_capHandle.IsValid())
                return;

            Addressables.Release(_capHandle);
            while (_capHandle.IsValid())
                await Task.Yield();
        }
    }
}