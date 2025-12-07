using UnityEngine;
using DG.Tweening;
using Core.Events;
using Core.GamePlay;
using UnityEngine.UI;

namespace Core.Store
{
    public class ItemView : MonoBehaviour
    {
        public ItemData MyData;

        [SerializeField] protected SOIntegerEvents ChangeItemStatusEvent;
        [SerializeField] protected Image SelectionObj;

        [SerializeField] Color32 SelectedImg, ActiveImg;

        Color32 _unSelectImg = Color.white;
        float _tweenTime = 0.25f;

        public virtual void UnSelectItem()
        {
            SelectionObj.DOColor(_unSelectImg, _tweenTime);
        }

        public virtual void SelectItem()
        {
            SelectionObj.DOColor(SelectedImg, _tweenTime);
        }

        public virtual void ActiveSelectItem()
        {
            SelectionObj.DOColor(ActiveImg, _tweenTime);
            MyData.IsPurchased = true;
        }
    }
}
