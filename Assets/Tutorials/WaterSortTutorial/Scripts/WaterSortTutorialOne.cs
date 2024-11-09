using UnityEngine;
using Core.Events;
using Core.Variables;
using System.Collections;

namespace Core.GamePlay.WaterSort
{
    public class WaterSortTutorialOne : MonoBehaviour
    {
        [SerializeField] SOInterger CanPlay, LevelCompleteStateIndex;
        [SerializeField] SOIntegerEvents ChangeStateEvent;
        [SerializeField] SOEvents SwipeColorsModeEvent;
        [SerializeField] CapsuleCollider MyCollider, OtherCollider;
        [SerializeField] Liquid MyLiquid;
        [SerializeField] GameObject HandObj, InfoTextObj;
        [SerializeField] Color CurrentColor;
        [SerializeField] int TubeCounter;

        private void OnEnable()
        {
            ChangeStateEvent.EventHandler += HideInfoText;
        }

        private void OnDisable()
        {
            ChangeStateEvent.EventHandler -= HideInfoText;
        }

        private void Start()
        {
            StartCoroutine(SetColors());
        }

        IEnumerator SetColors()
        {
            yield return new WaitForSeconds(0.5f);
            for (int c=0; c<2; c++)
            {
                MyLiquid.SetColor(CurrentColor);
                yield return new WaitForSeconds(0.1f);
            }
            if (TubeCounter == 1)
            {
                MyCollider.enabled = true;
                SwipeColorsModeEvent.InvokeSOEvent();
                CanPlay.Value = 1;
                HandObj.SetActive(true);
            }
        }

        private void OnMouseDown()
        {
            if (CanPlay.Value == 1)
            {
                if (TubeCounter == 1)
                {
                    MyCollider.enabled = false;
                    OtherCollider.enabled = true;
                    LeanTween.moveLocalX(HandObj, 130f, 0.35f).setEase(LeanTweenType.easeInOutBack);
                }
                else if (TubeCounter == 2)
                {
                    MyCollider.enabled = false;
                    HandObj.SetActive(false);
                }
            }
        }

        void HideInfoText(int stateNum)
        {
            if (LevelCompleteStateIndex.Value == stateNum)
                InfoTextObj.SetActive(false);
        }
    }
}
