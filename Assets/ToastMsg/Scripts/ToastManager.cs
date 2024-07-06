using UnityEngine;
using Core.Events;
using Core.Variables;

namespace Core.ToastMsg
{
    [CreateAssetMenu(fileName = "ToastManager", menuName = "ScriptableObjects/Toast/Manager")]
    public class ToastManager : ScriptableObject
    {
        [SerializeField] SOIntegerEvents ToastMsgEvent;
        [SerializeField] ToastScreen ToastMsgPrefab;
        [SerializeField] SOInterger OldMsgNum;

        ToastScreen _oldMsgScreen;

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
                OldMsgNum.Value = toastNum;
            }
            else if(toastNum != OldMsgNum.Value)
            {
                _oldMsgScreen.ChangeMsg(toastNum);
                OldMsgNum.Value = toastNum;
            }
        }
    }
}
