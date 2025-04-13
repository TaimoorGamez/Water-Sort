using UnityEngine;
using Core.Events;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField] SOEvents SelfDestructionEvent;
    [SerializeField] float BlasTime;

    private void OnEnable()
    {
        SelfDestructionEvent.EventHandler += BlastNow;
    }

    private void OnDisable()
    {
        SelfDestructionEvent.EventHandler -= BlastNow;
    }

    void BlastNow()
    {
        Destroy(gameObject, BlasTime);
    }
}
