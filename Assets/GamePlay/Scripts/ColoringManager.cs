using DG.Tweening;
using UnityEngine;
using Core.Events;
using Core.DB.Variables;

namespace Core.GamePlay.Coloring
{
    public class ColoringManager : MonoBehaviour
    {
        [SerializeField] SOEvents StartColoringEvent;
        [SerializeField] SOIntegerEvents SoundEffectEvent;
        [SerializeField] RectTransform ColoringImage;
        [SerializeField] Transform RefferanceBar;
        [SerializeField] DBInt LevelIndex;

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
            ColoringImage.DOAnchorPos(Vector2.zero, _preparationTime).OnComplete(() =>
            {
                RefferanceBar.gameObject.SetActive(true);
                Instantiate(Resources.Load(_refferancePath + LevelIndex.Value),RefferanceBar.GetChild(0));
            });
        }
    }
}
