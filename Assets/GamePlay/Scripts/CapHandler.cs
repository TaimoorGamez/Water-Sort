using UnityEngine;
using Core.DB.Variables;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.GamePlay.WaterSort
{
    public class CapHandler : MonoBehaviour
    {
        [SerializeField] DBInt CurrentCap;
        
        string _capsPath = "GamePlay/Cap/";
        CapAnimation _myAnimation;

        private void Start()
        {
            LoadCapItem(_capsPath + CurrentCap.Value);
        }

        async void LoadCapItem(string path)
        {
            AsyncOperationHandle<CapAnimation> capHandle = Addressables.LoadAssetAsync<CapAnimation>(path);
            await capHandle.Task;

            if (capHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Failed to load Addressable prefab at: {path}");
                return;
            }

            _myAnimation = null;

            _myAnimation = Instantiate(capHandle.Result, transform);

            await Task.Yield();
            await Task.Yield();

            while (_myAnimation == null)
                await Task.Yield();

            Addressables.Release(capHandle);
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
    }
}