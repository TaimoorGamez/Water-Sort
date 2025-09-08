using UnityEngine;

namespace Core.DailyTasks
{
    [System.Serializable]
    public class DailyTaskData
    {
        [SerializeField] int TaskId;

        public string Description;   
        public int Goal;

        public int Progress
        {
            get => PlayerPrefs.GetInt("Progress" + TaskId, 0);
            set
            {
                if (value > -1)
                {
                    if (value > Goal)
                        value = Goal;

                    PlayerPrefs.SetInt("Progress" + TaskId, value);
                    PlayerPrefs.Save();
                }
            }
        }

        public int TaskClaimed
        {
            get => PlayerPrefs.GetInt("TaskClaimed" + TaskId, 0);
            set
            {
                if (value == 0 || value == 1)
                {
                    PlayerPrefs.SetInt("TaskClaimed" + TaskId, value);
                    PlayerPrefs.Save();
                }
            }
        }
    }
}