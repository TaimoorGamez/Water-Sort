using UnityEngine;
using Core.Events;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField] float BlasTime;

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
        Destroy(gameObject, BlasTime);
    }
}
