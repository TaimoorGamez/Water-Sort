using UnityEngine;
using Core.Events;
using Core.DB.Variables;
using System.Collections.Generic;

namespace Core.DailyTasks
{
    [CreateAssetMenu(fileName = "DailyTaskManager", menuName = "ScriptableObjects/DaiyTask/TaskManager")]
    public class TaskManager : ScriptableObject
    {
        [SerializeField] SOEvents GenerateDailyTasksEvent;
        [SerializeField] DailyTaskData[] AllTasks;
        [SerializeField] DBInt[] TaskIndexs, TaskProgress, TaskClaimed;

        int _totalTasks = 4;
        DailyTaskData[] _activeTasks;

        private void OnEnable()
        {
            RetrevePreviousTasks();
            GenerateDailyTasksEvent.EventHandler += GenerateDailyTasks;
        }

        private void OnDisable()
        {
            GenerateDailyTasksEvent.EventHandler -= GenerateDailyTasks;
        }

        void GenerateDailyTasks()
        {
            _activeTasks = new DailyTaskData[_totalTasks];
            TaskIndexs[0].Value = 0;
            TaskProgress[0].Value = 0;
            _activeTasks[0] = AllTasks[0];
            _activeTasks[0].Progress = 0;

            List<int> availableIndexes = new List<int>();
            for (int i = 1; i < AllTasks.Length; i++)
            { availableIndexes.Add(i); }


            for (int i = 1; i < _totalTasks; i++)
            {
                int rand = Random.Range(0, availableIndexes.Count);
                int chosenIndex = availableIndexes[rand];

                TaskIndexs[i].Value = chosenIndex;
                TaskProgress[i].Value = 0;
                _activeTasks[i] = AllTasks[chosenIndex];
                _activeTasks[i].Progress = 0;
                availableIndexes.RemoveAt(rand);
            }
        }

        void RetrevePreviousTasks()
        {
            _activeTasks = new DailyTaskData[_totalTasks];
            for (int i = 0; i < _totalTasks; i++)
            {
                int index = TaskIndexs[i].Value;
                _activeTasks[i] = AllTasks[index];
                _activeTasks[i].Progress = TaskProgress[i].Value;
                Debug.Log(i);
            }
        }

        public DailyTaskData[] GetActiveTasks()
        {
            return _activeTasks;
        }

    }
}
