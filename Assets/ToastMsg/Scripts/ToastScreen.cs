using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Core.ToastMsg
{
    public class ToastScreen : MonoBehaviour
    {
        [SerializeField] Text MsgText;
        [SerializeField] string[] ToastMsgs;

        float _destroyDelay = 5;
        Coroutine _sefDestructionRotine;

        private void Start()
        {
            _sefDestructionRotine = StartCoroutine(SelfDestruct());
        }

        public void ChangeMsg(int msgNum)
        {
            _destroyDelay = 5;
            MsgText.text = ToastMsgs[msgNum];
            MsgText.transform.parent.gameObject.SetActive(false);
            MsgText.transform.parent.position = Vector3.zero;
            MsgText.transform.parent.gameObject.SetActive(true);
        }

        IEnumerator SelfDestruct()
        {
            while (_destroyDelay > 0)
            {
                _destroyDelay -= Time.deltaTime;
                yield return null;
            }
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            if (_sefDestructionRotine != null)
            {
                StopCoroutine(_sefDestructionRotine);
            }
        }
    }
}
