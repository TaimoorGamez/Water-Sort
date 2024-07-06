using UnityEngine;
using Core.Events;
using Core.Variables;
using System.Collections;

namespace Core.GamePlay.WaterSort
{
    public class WaterSortTutorialOne : MonoBehaviour
    {
        [SerializeField] SOInterger CanPlay;
        [SerializeField] SOEvents SwipeColorsModeEvent;
        [SerializeField] CapsuleCollider MyCollider, OtherCollider;
        [SerializeField] WaterColor MyLiquid;
        [SerializeField] GameObject HandObj;
        [SerializeField] Color CurrentColor;
        [SerializeField] int TubeCounter;

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
                SwipeColorsModeEvent.InvokeEvent();
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
                    LeanTween.moveX(HandObj, 1.6f, 0.35f).setEase(LeanTweenType.easeInOutBack);
                }
                else if (TubeCounter == 2)
                {
                    MyCollider.enabled = false;
                    HandObj.SetActive(false);
                }
            }
        }
    }
}
