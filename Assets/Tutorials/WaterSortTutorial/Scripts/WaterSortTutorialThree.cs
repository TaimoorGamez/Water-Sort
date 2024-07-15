using UnityEngine;
using Core.Events;
using Core.Variables;
using Core.Animations.LT;
using System.Collections;

namespace Core.GamePlay.WaterSort
{
    public class WaterSortTutorialThree : MonoBehaviour
    {
        [SerializeField] SOLeanTween ScaleDownButton;
        [SerializeField] SOEvents SwipeColorsModeEvent;
        [SerializeField] SOInterger CanPlay;
        [SerializeField] WaterColor FirstLiquid, SecondLiquid, ThirdLiquid;
        [SerializeField] GameObject HandObj, ExtraTube, InfoText;
        [SerializeField] Color[] CurrentColors;

        int _colorIndex = 0;

        private void Start()
        {
            StartCoroutine(SetColors());
            ScaleDownButton.TargetObj = this.gameObject;
        }

        IEnumerator SetColors()
        {
            yield return new WaitForSeconds(0.5f);
            for (int c = 0; c < 4; c++)
            {
                FirstLiquid.SetColor(CurrentColors[_colorIndex]);
                yield return new WaitForSeconds(0.1f);
                if (_colorIndex == 2)
                {
                    _colorIndex = 0;
                }
                else
                {
                    _colorIndex++;
                }
                SecondLiquid.SetColor(CurrentColors[_colorIndex]);
                yield return new WaitForSeconds(0.1f); 
                if (_colorIndex == 2)
                {
                    _colorIndex = 0;
                }
                else
                {
                    _colorIndex++;
                }
                ThirdLiquid.SetColor(CurrentColors[_colorIndex]);
                yield return new WaitForSeconds(0.1f);
            }
            SwipeColorsModeEvent.InvokeSOEvent();
            CanPlay.Value = 1;
            HandObj.SetActive(true);
        }

        private void OnMouseDown()
        {
            if (CanPlay.Value == 1)
            {
                ScaleDownButton.PlayAnimation();
                ExtraTube.SetActive(true);
                HandObj.SetActive(false);
                InfoText.SetActive(true);
                Destroy(gameObject);
            }
        }
    }
}
