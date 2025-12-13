using TMPro;
using Core.Store;
using UnityEngine;
using DG.Tweening;
using Core.Events;
using Core.Variables;
using Core.DB.Variables;
using System.Collections;
using System.Collections.Generic;

namespace Core.Screen
{
    public class StorageRoomView : MonoBehaviour
    {
        [SerializeField] SODictionary_Int_Gameobject ItemInstances;
        [SerializeField] DBInt CurrentActiveItem;
        [SerializeField] ItemView[] RoomItems;
        [SerializeField] RectTransform Content, Viewport;
        [SerializeField] GameObject[] Buttons;
        [SerializeField] Transform ItemHolder;
        [SerializeField] TextMeshProUGUI VideoText;
        [SerializeField] string BuyEventName;

        int _selectedItem = -1;
        Coroutine _activationRotine;
        GameObject _currentItem;

        private void OnEnable()
        {
            if (EventDictionariesHolder.StoreBuyEvents.TryGetValue(BuyEventName, out var evt))
            {
                evt += PurchaseByAd;
            }
            else
            {
                Debug.Log($"Buy event key '{BuyEventName}' does not exist.");
            }
            SingleIntegerEventsHolder.UpdateItemStatusEvent += ChangeItemStatus;
            _selectedItem = CurrentActiveItem.Value;
            _activationRotine = StartCoroutine(ActiveStorageRoom());
            ChangeItemStatus(_selectedItem);
        }

        private void OnDisable()
        {
            if (EventDictionariesHolder.StoreBuyEvents.TryGetValue(BuyEventName, out var evt))
            {
                evt -= PurchaseByAd;
            }
            else
            {
                Debug.Log($"Buy event key '{BuyEventName}' does not exist.");
            }
            SingleIntegerEventsHolder.UpdateItemStatusEvent -= ChangeItemStatus;
            StopActiveRotines();
            if (_currentItem != null)
            {
                Destroy(_currentItem);
            }
            for (int i = 0; i < RoomItems.Length; i++)
            {
                RoomItems[i].DOKill();
                RoomItems[i].transform.localScale = Vector3.zero; 
                RoomItems[i].gameObject.SetActive(false);
            }
        }

        IEnumerator ActiveStorageRoom()
        {
            if (ItemInstances.DictionaryValue == null)
            {
                ItemInstances.DictionaryValue = new Dictionary<int, GameObject>();
            }

            for (int i = 0; i < RoomItems.Length; i++)
            {
                RoomItems[i].gameObject.SetActive(true);
                yield return new WaitForSeconds(0.1f);
            }
            ChangeActiveItem();
            InitItem(CurrentActiveItem.Value);
            yield return new WaitForSeconds(0.5f);
            float targetY = Content.rect.height - Viewport.rect.height;
            if (targetY > 200)
            {
                Content.DOAnchorPosY(targetY, 0.25f).SetEase(Ease.OutBack);
            }
            StopActiveRotines();
        }

        void StopActiveRotines()
        {
            if (_activationRotine != null)
            {
                StopCoroutine(_activationRotine);
                _activationRotine = null;
            }
        }

        void InitItem(int item)
        {
            if (_currentItem != null)
            {
                Destroy(_currentItem);
            }
            _currentItem = Instantiate(ItemInstances.DictionaryValue[item], ItemHolder);
        }

        void ChangeItemStatus(int selectedItem)
        {
            _selectedItem = selectedItem;
            for (int i = 0; i < RoomItems.Length; i++)
            {
                if (i == selectedItem)
                {
                    RoomItems[i].SelectItem();
                    InitItem(i);
                    if (i == CurrentActiveItem.Value)
                    {
                        ChangeButton(0);
                    }
                    else if (RoomItems[i].MyData.IsPurchased)
                    {
                        ChangeButton(1);
                    }
                    else
                    {
                        ChangeButton(2);
                        VideoText.text = RoomItems[_selectedItem].MyData.WatchedVideos + "/" + RoomItems[_selectedItem].MyData.TotalVideos;
                    }
                }
                else if (i == CurrentActiveItem.Value)
                {
                    RoomItems[i].ActiveSelectItem();
                }
                else
                {
                    RoomItems[i].UnSelectItem();
                }
            }
        }

        void ChangeButton(int activeButton)
        {
            for (int b = 0; b < Buttons.Length; b++)
            {
                Buttons[b].SetActive(false);
            }
            Buttons[activeButton].SetActive(true);
        }

        public void ChangeActiveItem()
        {
            RoomItems[CurrentActiveItem.Value].UnSelectItem();
            if (_selectedItem != -1)
            {
                CurrentActiveItem.Value = _selectedItem;
                RoomItems[CurrentActiveItem.Value].ActiveSelectItem();
                ChangeButton(0);
            }
        }

        void PurchaseByAd()
        {
            RoomItems[_selectedItem].MyData.WatchedVideos += 1;
            VideoText.text = RoomItems[_selectedItem].MyData.WatchedVideos + "/" + RoomItems[_selectedItem].MyData.TotalVideos;
            if (RoomItems[_selectedItem].MyData.WatchedVideos >= RoomItems[_selectedItem].MyData.TotalVideos)
            {
                RoomItems[_selectedItem].MyData.IsPurchased = true;
                ChangeItemStatus(_selectedItem);
            }
        }
    }
}