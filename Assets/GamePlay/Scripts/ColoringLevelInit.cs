using UnityEngine;
using Core.Events;
using Core.Variables;
using Core.DB.Variables;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.GamePlay.WaterSort
{
    public class ColoringLevelInit : MonoBehaviour
    {
        [SerializeField] DBInt LvlIndex;
        [SerializeField] SOInterger TempLevelIndex;
        [SerializeField] Transform ColoringHolder;
         
        string _coloringPath = "Level/Coloring/";
        AsyncOperationHandle _coloringHandle;


        private void OnEnable()
        {
            SimpleEventsHolder.InitLvlEvent += InitColoring;
            SimpleEventsHolder.RestartLevelEvent += RegenrateColoring;
            SimpleEventsHolder.DestroyLevelEvent += DestroyColoring;
        }

        private async void OnDisable()
        {
            SimpleEventsHolder.InitLvlEvent -= InitColoring;
            SimpleEventsHolder.RestartLevelEvent -= RegenrateColoring;
            SimpleEventsHolder.DestroyLevelEvent -= DestroyColoring;
            await ReleaseHandler();
        }

        void InitColoring()
        {
            LoadColoringLvl(_coloringPath + (TempLevelIndex.Value == -1 ? LvlIndex.Value : TempLevelIndex.Value));
        }

        async void LoadColoringLvl(string path)
        {
            _coloringHandle = Addressables.LoadAssetAsync<GameObject>(path);
            await _coloringHandle.Task;

            if (_coloringHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Failed to load Addressable prefab at: {path}");
                return;
            }

            GameObject lvlObj = Instantiate(_coloringHandle.Result as GameObject, ColoringHolder);

            await Task.Yield();
            await Task.Yield();

            while (lvlObj == null)
                await Task.Yield();
        }

        void RegenrateColoring()
        {
            DestroyColoring();
            InitColoring();
        }

        async void DestroyColoring()
        {
            await ReleaseHandler();
            Destroy(ColoringHolder.GetChild(0).gameObject);
        }

        async Task ReleaseHandler()
        {
            if(!_coloringHandle.IsValid())
                return;

            Addressables.Release(_coloringHandle);
            while (_coloringHandle.IsValid())
                await Task.Yield();
        }
    }
}
