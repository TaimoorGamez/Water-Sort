using Core.Events;
using DG.Tweening;
using UnityEngine;
using Core.Variables;
using Core.DB.Variables;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.GamePlay.Coloring
{
    public class ColoringManager : MonoBehaviour
    {
        [SerializeField] DBInt LevelIndex;
        [SerializeField] SOInterger TempLevelIndex;
        [SerializeField] SOEvents StartColoringEvent;
        [SerializeField] SOIntegerEvents SoundEffectEvent;
        [SerializeField] RectTransform ColoringImage;
        [SerializeField] Transform RefferanceBar;

        float _preparationTime = 1;
        string _refferancePath = "Level/Reference/";
        AsyncOperationHandle _referenceHandle;

        private void OnEnable()
        {
            StartColoringEvent.EventHandler += StartColoring;
        }

        private async void OnDisable()
        {
            StartColoringEvent.EventHandler -= StartColoring;
            await ReleaseHandler();
        }

        void StartColoring()
        {
            SoundEffectEvent.InvokeSOEvent(3);
            ColoringImage.DOScale(Vector3.one, _preparationTime);
            ColoringImage.DOAnchorPos(Vector2.zero, _preparationTime).OnComplete(() =>
            {
                RefferanceBar.gameObject.SetActive(true);
                LoadReferenceObj(_refferancePath + (TempLevelIndex.Value == -1 ? LevelIndex.Value : TempLevelIndex.Value));
            });
        }

        async void LoadReferenceObj(string path)
        {
            _referenceHandle = Addressables.LoadAssetAsync<GameObject>(path);
            await _referenceHandle.Task;

            if (_referenceHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Failed to load Addressable prefab at: {path}");
                return;
            }

            GameObject referenceObj = Instantiate(_referenceHandle.Result as GameObject, RefferanceBar.GetChild(0));

            await Task.Yield();
            await Task.Yield();

            while (referenceObj == null)
                await Task.Yield();
        }

        async Task ReleaseHandler()
        {
            if(!_referenceHandle.IsValid())
                return;

            Addressables.Release(_referenceHandle);
            while (_referenceHandle.IsValid())
                await Task.Yield();
        }
    }
}
