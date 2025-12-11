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
        [SerializeField] SOEvents InitLevelEvent, RestartLevelEvent, DestroyLevelEvent;
        [SerializeField] DBInt LvlIndex;
        [SerializeField] SOInterger TempLevelIndex;
        [SerializeField] Transform ColoringHolder;
         
        string _coloringPath = "Level/Coloring/";

        
        private void OnEnable()
        {
            InitLevelEvent.EventHandler += InitColoring;
            RestartLevelEvent.EventHandler += RegenrateColoring;
            DestroyLevelEvent.EventHandler += DestroyColoring;
        }

        private void OnDisable()
        {
            InitLevelEvent.EventHandler -= InitColoring;
            RestartLevelEvent.EventHandler -= RegenrateColoring;
            DestroyLevelEvent.EventHandler -= DestroyColoring;
        }

        void InitColoring()
        {
            LoadColoringLvl(_coloringPath + (TempLevelIndex.Value == -1 ? LvlIndex.Value : TempLevelIndex.Value));
        }

        async void LoadColoringLvl(string path)
        {
            AsyncOperationHandle<GameObject> coloringHandle = Addressables.LoadAssetAsync<GameObject>(path);
            await coloringHandle.Task;

            if (coloringHandle.Status != AsyncOperationStatus.Succeeded)
            {
                Debug.LogError($"Failed to load Addressable prefab at: {path}");
                return;
            }

            GameObject lvlObj = Instantiate(coloringHandle.Result, ColoringHolder);

            await Task.Yield();
            await Task.Yield();

            while (lvlObj == null)
                await Task.Yield();

            Addressables.Release(coloringHandle);
        }

        void RegenrateColoring()
        {
            DestroyColoring();
            InitColoring();
        }
        void DestroyColoring()
        {
            Destroy(ColoringHolder.GetChild(0).gameObject);
        }
    }
}
