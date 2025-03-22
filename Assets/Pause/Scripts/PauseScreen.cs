using UnityEngine;
using DG.Tweening;
using Core.Events;
using Core.Variables;

namespace Core.Screen
{
    public class PauseScreen : MonoBehaviour
    {
        [SerializeField] SOIntegerEvents DestroyStatEvent, ActiveStatEvent;
        [SerializeField] SOEvents RestartLevelEvent, DestroyLevelEvent;
        [SerializeField] SOInterger CanPlay, MainMenuStateIndex, GamePlayStateIndex;
        [SerializeField] Transform Body;

        float _tweenTime = 0.25f;

        private void OnEnable()
        {
            Body.DOScale(1, _tweenTime).SetEase(Ease.OutBack);
        }

        public void RestartLevel()
        {
            RestartLevelEvent.InvokeSOEvent();
            ClosePanel();
        }

        public void ClosePanel()
        {
            Body.DOScale(0, _tweenTime).SetEase(Ease.InBack).OnComplete(() => {
                CanPlay.Value = 1;
                Destroy(gameObject);
            });
        }

        public void GoHome()
        {
            DestroyLevelEvent.InvokeSOEvent();
            DestroyStatEvent.InvokeSOEvent(GamePlayStateIndex.Value);
            ActiveStatEvent.InvokeSOEvent(MainMenuStateIndex.Value);
            ClosePanel();
        }
    }
}
