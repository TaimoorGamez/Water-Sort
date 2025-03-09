using TMPro;
using UnityEngine;
using System.Collections;

namespace Core.ToastMsg
{
    public class ToastScreen : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI MsgText;
        [SerializeField] string[] ToastMsgs;

        float _destroyDelay = 3;
        Coroutine _selfDestructionRotine;

        private void Start()
        {
            _selfDestructionRotine = StartCoroutine(SelfDestruct());
        }

        public void ChangeMsg(int msgNum)
        {
            _destroyDelay = 3;
            MsgText.text = ToastMsgs[msgNum];
            MsgText.transform.parent.gameObject.SetActive(false);
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
            if (_selfDestructionRotine != null)
            {
                StopCoroutine(_selfDestructionRotine);
            }
        }
    }
}
