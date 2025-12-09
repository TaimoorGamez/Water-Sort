using Core.Events;
using DG.Tweening;
using UnityEngine;
using Core.Variables;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.Screen
{
    public class StoreScreen : UiScreens
    {
        [SerializeField] SOIntegerEvents SoundEffectEvent;
        [SerializeField] Transform FillImage;
        [SerializeField] SOAsyncIList StoreItemsList;

        string _storeItemsLabel = "store_item";

        private void OnEnable()
        {
            LoadStoreItems();
        }

        public override void OnOpen()
        {
            SoundEffectEvent.InvokeSOEvent(3);
            Body.DOAnchorPosX(0, _transitionDuration).SetEase(Ease.OutBack);
        }

        async void LoadStoreItems()
        {
            Clear();
            StoreItemsList.ValueList = new AsyncOperationHandle<IList<GameObject>>();
            StoreItemsList.ValueList = Addressables.LoadAssetsAsync<GameObject>(_storeItemsLabel, null);
            await StoreItemsList.ValueList.Task;
            if (StoreItemsList.ValueList.Status == AsyncOperationStatus.Succeeded)
                OnOpen();
            else
                OnClose();

        }

        public override void OnClose()
        {
            SoundEffectEvent.InvokeSOEvent(2);
            Body.DOAnchorPosX(1500, _transitionDuration/2).SetEase(Ease.InBack).OnComplete(()=> gameObject.SetActive(false));
        }

        void Clear()
        {
            if (StoreItemsList.ValueList.IsValid())
                Addressables.Release(StoreItemsList.ValueList);

            StoreItemsList.ValueList = default;
        }
    }
}
