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
        bool _isInit = false;

        public void InitToastMsg()
        {
            ToastMsgEvent.EventHandler += ShowToastMsg;
            _isInit = true;
        }

        private void OnDisable()
        {
            if (_isInit)
            {
                ToastMsgEvent.EventHandler -= ShowToastMsg;
            } 
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
