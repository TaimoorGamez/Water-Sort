using UnityEngine;
using Core.Events;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField] float DelayTime;

    private void OnEnable()
    {
        SimpleEventsHolder.SelfDestructionEvent += BlastNow;
    }

    private void OnDisable()
    {
        SimpleEventsHolder.SelfDestructionEvent -= BlastNow;
    }

    void BlastNow()
    {
        Destroy(gameObject, DelayTime);
    }
}
