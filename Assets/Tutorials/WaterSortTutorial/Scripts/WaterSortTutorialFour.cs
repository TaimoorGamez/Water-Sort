using UnityEngine;
using Core.Events;
using Core.Variables;
using Core.Animations.LT;
using System.Collections;

namespace Core.GamePlay.WaterSort
{
    public class WaterSortTutorialFour : MonoBehaviour
    {
        [SerializeField] SOLeanTween ScaleDownButton, ScaleUpButton;
        [SerializeField] SOEvents SwipeColorsModeEvent;
        [SerializeField] SOInterger CanPlay, IsSwaping, UsingAnyFeature;
        [SerializeField] CapsuleCollider ColliderOne, ColliderTwo, ColliderThree, ColliderFour, ColliderFive;
        [SerializeField] WaterColor FirstLiquid, SecondLiquid, ThirdLiquid, ForthLiquid;
        [SerializeField] GameObject HandObj, LockOne, LockTwo;
        [SerializeField] Color[] CurrentColors;
        [SerializeField] bool IsSwipButton, IsFirstTube, IsLastTube;

        int _colorIndex = 0;
        bool _isFirstClick = true;

        private void Start()
        {
            if (IsSwipButton)
            { 
                StartCoroutine(SetColors());
                ScaleDownButton.TargetObj = this.gameObject;
                ScaleUpButton.TargetObj = this.gameObject;
            }
        }

        IEnumerator SetColors()
        {
            yield return new WaitForSeconds(0.5f);
            for (int c = 0; c < 4; c++)
            {
                FirstLiquid.SetColor(CurrentColors[_colorIndex]);
                yield return new WaitForSeconds(0.1f);
                if (_colorIndex == 3)
                {
                    _colorIndex = 0;
                }
                else
                {
                    _colorIndex++;
                }
                SecondLiquid.SetColor(CurrentColors[_colorIndex]);
                yield return new WaitForSeconds(0.1f); 
                if (_colorIndex == 3)
                {
                    _colorIndex = 0;
                }
                else
                {
                    _colorIndex++;
                }
                ThirdLiquid.SetColor(CurrentColors[_colorIndex]);
                yield return new WaitForSeconds(0.1f); 
                if (_colorIndex == 3)
                {
                    _colorIndex = 0;
                }
                else
                {
                    _colorIndex++;
                }
                ForthLiquid.SetColor(CurrentColors[_colorIndex]);
            }
            SwipeColorsModeEvent.InvokeSOEvent();
            CanPlay.Value = 1;
            HandObj.SetActive(true);
        }

        private void OnMouseDown()
        {
            if (CanPlay.Value == 1)
            {
                if (IsSwipButton && UsingAnyFeature.Value == 0 && IsSwaping.Value == 0)
                {
                    ScaleUpButton.CancleAnimation();
                    ScaleDownButton.PlayAnimation();
                    UsingAnyFeature.Value = 1;
                    SwipeColorsModeEvent.InvokeSOEvent();
                    IsSwaping.Value = 1;
                    if (_isFirstClick)
                    {
                        _isFirstClick = false;
                        LeanTween.moveLocalY(LockOne, -1, 1).setEase(LeanTweenType.easeInBack).setOnComplete(() => { LockOne.SetActive(false); });
                        LeanTween.move(HandObj, new Vector3(2.6f, 4, 0), 0.35f).setEase(LeanTweenType.easeInOutBack);
                        ColliderThree.enabled = true;
                    }
                }
                else if (IsFirstTube)
                {
                    if (_isFirstClick)
                    {
                        _isFirstClick = false;
                        LeanTween.moveLocalY(LockOne, -1, 1).setEase(LeanTweenType.easeInBack).setOnComplete(() => { LockOne.SetActive(false); });
                        LeanTween.moveX(HandObj, -0.35f, 0.35f).setEase(LeanTweenType.easeInOutBack);
                        ColliderThree.enabled = false;
                        ColliderTwo.enabled = true;
                        enabled = false;
                    }
                }
                else if (IsLastTube)
                {
                    if (_isFirstClick)
                    {
                        _isFirstClick = false;
                        LeanTween.moveLocalY(LockOne, -1, 1).setEase(LeanTweenType.easeInBack).setOnComplete(() => { LockOne.SetActive(false); });
                        LeanTween.moveLocalY(LockTwo, -1, 1).setEase(LeanTweenType.easeInBack).setOnComplete(() => { LockTwo.SetActive(false); });
                        Destroy(HandObj);
                        ColliderOne.enabled = true;
                        ColliderTwo.enabled = true;
                        ColliderThree.enabled = true;
                        ColliderFour.enabled = true;
                        ColliderFive.enabled = true;
                    }
                }
            }
        }

        private void OnMouseUp()
        {
            if (IsSwipButton)
            {
                ScaleDownButton.CancleAnimation();
                ScaleUpButton.PlayAnimation();
            }
        }
    }
}
