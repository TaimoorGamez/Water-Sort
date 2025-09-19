using DG.Tweening;
using UnityEngine;
using Core.Events;
using Core.Variables;
using Core.DB.Variables;

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
        string _refferancePath = "RefferanceImges/lvl ";

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
                Instantiate(Resources.Load(_refferancePath + (TempLevelIndex.Value == -1 ? LevelIndex.Value : TempLevelIndex.Value)), RefferanceBar.GetChild(0));
            });
        }
    }
}
