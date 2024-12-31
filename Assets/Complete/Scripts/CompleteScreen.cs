using UnityEngine;
using Core.Events;
using Core.Animations.DT;

public class CompleteScreen : MonoBehaviour
{
    [SerializeField] SOIntegerEvents SoundEffectEvent;
    [SerializeField] SOEvents DestroyLevelEvent;
    [SerializeField] SOAnChorMove ShowPanel;
    [SerializeField] GameObject Body;

    private void OnEnable()
    {
        DestroyLevelEvent.InvokeSOEvent();
        ShowPanel.TargetObj = Body;
        ShowPanel.PlayAnimation();
        SoundEffectEvent.InvokeSOEvent(3);
    }


    void OnClickNexxt()
    {
        //Debug.Log("Here30");
        //CanPlay.Value = 0;
        //ChangeStateEvent.InvokeEvent(LevelCompleteStateIndex.Value);
        //LvlNum.Value++;
    }
}
