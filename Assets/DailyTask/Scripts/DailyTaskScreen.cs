using DG.Tweening;
using UnityEngine;
using Core.Events;
using Core.DailyTasks;

namespace Core.Screen
{
    public class DailyTaskScreen : MonoBehaviour
    {
        [SerializeField] SOIntegerEvents SoundEffectEvent;
        [SerializeField] TaskManager CurrenTaskManager;
        [SerializeField] RectTransform Body;
        [SerializeField] TaskBar[] TaskBars;

        DailyTaskData[] _activeTasks;

        float _tweenTime = 0.25f;

        void OnEnable()
        {
            Body.DOScale(1, _tweenTime).SetEase(Ease.OutBack);
            SoundEffectEvent.InvokeSOEvent(2);
            UpdateTasks();
        }

        private void UpdateTasks()
        {
            _activeTasks = CurrenTaskManager.GetActiveTasks();

            for (int i = 0; i < TaskBars.Length; i++)
            {
                TaskBars[i].SetTask(_activeTasks[i]);
            }
        }

        public void ClosePanel()
        {
            Body.DOScale(0, _tweenTime).SetEase(Ease.InBack).OnComplete(() => Destroy(gameObject));
            SoundEffectEvent.InvokeSOEvent(2);
        }
    }
}
