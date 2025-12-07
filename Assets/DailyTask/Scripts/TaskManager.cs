using UnityEngine;
using Core.Events;
using Core.DB.Variables;
using System.Collections.Generic;

namespace Core.DailyTasks
{
    [CreateAssetMenu(fileName = "DailyTaskManager", menuName = "ScriptableObjects/DaiyTask/TaskManager")]
    public class TaskManager : ScriptableObject
    {
        [SerializeField] SO2IntergerEvent TaskEvent;
        [SerializeField] SOEvents GenerateDailyTasksEvent;
        [SerializeField] DailyTaskData[] AllTasks;
        [SerializeField] DBInt[] TaskIndexs;

        int _totalTasks = 4;
        DailyTaskData[] _activeTasks;

        private void OnEnable()
        {
            RetrevePreviousTasks();
            GenerateDailyTasksEvent.EventHandler += GenerateDailyTasks;
            TaskEvent.EventHandler += AddTaskProgress;
        }

        private void OnDisable()
        {
            GenerateDailyTasksEvent.EventHandler -= GenerateDailyTasks;
            TaskEvent.EventHandler -= AddTaskProgress;
        }

        void GenerateDailyTasks()
        {
            _activeTasks = new DailyTaskData[_totalTasks];
            TaskIndexs[0].Value = 0;
            AllTasks[0].Progress = 0;
            AllTasks[0].TaskClaimed = 0;
            _activeTasks[0] = AllTasks[0];

            List<int> availableIndexes = new List<int>();
            for (int i = 1; i < AllTasks.Length; i++)
            {
                AllTasks[i].Progress = 0;
                AllTasks[i].TaskClaimed = 0;
                availableIndexes.Add(i); 
            }


            for (int i = 1; i < _totalTasks; i++)
            {
                int rand = Random.Range(0, availableIndexes.Count);
                int chosenIndex = availableIndexes[rand];

                TaskIndexs[i].Value = chosenIndex;
                _activeTasks[i] = AllTasks[chosenIndex];
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
            }
        }

        public DailyTaskData[] GetActiveTasks()
        {
            return _activeTasks;
        }

        void AddTaskProgress(int index, int progress)
        {
            for (int t = 0; t < _totalTasks; t++) 
            {
                if (TaskIndexs[t].Value == index)
                {
                    _activeTasks[t].Progress += progress;
                }
            }
        }
    }
}
