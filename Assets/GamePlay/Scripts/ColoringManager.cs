using UnityEngine;
using DG.Tweening;
using Core.Events;
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

        private void OnEnable()
        {
            StartColoringEvent.EventHandler += StartColoring;
        }

        private void OnDisable()
        {
            StartColoringEvent.EventHandler -= StartColoring;
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
            AsyncOperationHandle<GameObject> referenceHandle = Addressables.LoadAssetAsync<GameObject>(path);
            await referenceHandle.Task;

            if (referenceHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Failed to load Addressable prefab at: {path}");
                return;
            }

            GameObject referenceObj = Instantiate(referenceHandle.Result, RefferanceBar.GetChild(0));

            await Task.Yield();
            await Task.Yield();

            while (referenceObj == null)
                await Task.Yield();

            Addressables.Release(referenceHandle);
        }
    }
}
