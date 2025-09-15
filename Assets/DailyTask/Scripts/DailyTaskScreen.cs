using DG.Tweening;
using UnityEngine;
using Core.Events;
using Core.Economy;
using UnityEngine.UI;
using Core.DailyTasks;

namespace Core.Screen
{
    public class DailyTaskScreen : MonoBehaviour
    {
        [SerializeField] Currency CashCurrency;
        [SerializeField] SOIntegerEvents SoundEffectEvent;
        [SerializeField] TaskManager CurrenTaskManager;
        [SerializeField] RectTransform Body, BoxPanel;
        [SerializeField] TaskBar[] TaskBars;
        [SerializeField] RectTransform[] RewardImgs;
        [SerializeField] GameObject[] RewardChecks;
        [SerializeField] Image RewardFillBar;
        [SerializeField] GameObject NotificationObj;

        DailyTaskData[] _activeTasks;

        float _tweenTime = 0.25f;
        int _taskClaimed = 0;

        void OnEnable()
        {
            Body.DOScale(1, _tweenTime).SetEase(Ease.OutBack);
            SoundEffectEvent.InvokeSOEvent(2);
            UpdateTasks();
        }

        void UpdateTasks()
        {
            _activeTasks = CurrenTaskManager.GetActiveTasks();
            int _completedTasks = 0;
            for (int i = 0; i < TaskBars.Length; i++)
            {
                TaskBars[i].SetTask(_activeTasks[i]);
                if(_activeTasks[i].Progress >= _activeTasks[i].Goal)
                {
                    _completedTasks++;
                    if (_activeTasks[i].TaskClaimed == 1)
                    {
                        _completedTasks++;
                        RewardChecks[i].SetActive(true);
                        RewardImgs[i].gameObject.SetActive(false);
                    }
                    else
                    {
                        RewardChecks[i].SetActive(false);
                        RewardImgs[i].gameObject.SetActive(true);
                        RewardImgs[i].DOScale(1.25f, 0.5f)
                            .SetLoops(-1, LoopType.Yoyo)
                            .SetEase(Ease.InOutSine);
                    }
                }
                else
                {
                    RewardChecks[i].SetActive(false);
                    RewardImgs[i].gameObject.SetActive(true);
                }
            }

            RewardFillBar.fillAmount = (float)_completedTasks / TaskBars.Length;
        }

       public void ClaimReward(int taskIndex)
        {
            if(_activeTasks[taskIndex].Progress >= _activeTasks[taskIndex].Goal && _activeTasks[taskIndex].TaskClaimed == 0)
            {
                _activeTasks[taskIndex].TaskClaimed = 1;
                switch(_taskClaimed)
                {
                    case 0:
                        CashCurrency.Amount += 100;
                        break;
                    case 1:
                        CashCurrency.Amount += 150;
                        break;
                    case 2:
                        CashCurrency.Amount += 200;
                        break;
                    case 3:
                        BoxPanel.gameObject.SetActive(true);
                        break;
                }
                UpdateTasks();
            }
        }

        public void ClosePanel()
        {
            SoundEffectEvent.InvokeSOEvent(2);
            Body.DOScale(0, _tweenTime).SetEase(Ease.InBack).OnComplete(() => {
                NotificationObj.SetActive(false);
                gameObject.SetActive(false);
            });
        }
    }
}
