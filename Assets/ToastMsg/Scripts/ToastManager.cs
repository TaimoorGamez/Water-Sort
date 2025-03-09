using UnityEngine;
using Core.Events;

namespace Core.ToastMsg
{
    [CreateAssetMenu(fileName = "ToastManager", menuName = "ScriptableObjects/Toast/Manager")]
    public class ToastManager : ScriptableObject
    {
        [SerializeField] SOIntegerEvents ToastMsgEvent;
        [SerializeField] ToastScreen ToastMsgPrefab;

        ToastScreen _oldMsgScreen;
        int _oldMsgNum = -1;

        private void OnEnable()
        {
            ToastMsgEvent.EventHandler += ShowToastMsg;
        }

        private void OnDisable()
        {
            ToastMsgEvent.EventHandler -= ShowToastMsg;
        }

        void ShowToastMsg(int toastNum)
        {
            if (_oldMsgScreen == null)
            {
                _oldMsgScreen = Instantiate(ToastMsgPrefab);
                _oldMsgScreen.ChangeMsg(toastNum);
                _oldMsgNum = toastNum;
            }
            else if(toastNum != _oldMsgNum)
            {
                _oldMsgScreen.ChangeMsg(toastNum);
                _oldMsgNum = toastNum;
            }
        }
    }
}
