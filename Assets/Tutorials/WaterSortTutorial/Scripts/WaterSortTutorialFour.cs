using UnityEngine;
using Core.Events;
using UnityEngine.UI;
using Core.Variables;
using System.Collections;
using DG.Tweening;

namespace Core.GamePlay.WaterSort
{
    public class WaterSortTutorialFour : MonoBehaviour
    {
        [SerializeField] SOEvents SwipeColorsModeEvent;
        [SerializeField] SOInterger CanPlay, IsSwaping, UsingAnyFeature, LevelCompleteStateIndex;
        [SerializeField] SOIntegerEvents ChangeStateEvent;
        [SerializeField] CapsuleCollider ColliderOne, ColliderTwo, ColliderThree, ColliderFour, ColliderFive;
        [SerializeField] Liquid FirstLiquid, SecondLiquid, ThirdLiquid, ForthLiquid;
        [SerializeField] GameObject HandObj, InfoTextObj;
        [SerializeField] Button SwapeBtn;
        [SerializeField] Color[] CurrentColors;
        [SerializeField] bool IsSwipButton, IsFirstTube, IsLastTube;

        int _colorIndex = 0;
        bool _isFirstClick = true;

        private void OnEnable()
        {
            if (IsSwipButton)
            {
                ChangeStateEvent.EventHandler += HideInfoText;
                SwapeBtn.onClick.AddListener(SwapeColor);
            }
        }

        private void OnDisable()
        {
            if (IsSwipButton)
            {
                ChangeStateEvent.EventHandler -= HideInfoText;
                SwapeBtn.onClick.RemoveListener(SwapeColor);
            }
        }

        private void Start()
        {
            if (IsSwipButton)
            { 
                StartCoroutine(SetColors());
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
                if (IsFirstTube)
                {
                    if (_isFirstClick)
                    {
                        _isFirstClick = false;
                        HandObj.transform.DOLocalMoveX(-45f, 0.35f).SetEase(Ease.InOutBack);
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

        void SwapeColor()
        {
            if (UsingAnyFeature.Value == 0 && IsSwaping.Value == 0)
            {
                UsingAnyFeature.Value = 1;
                SwipeColorsModeEvent.InvokeSOEvent();
                IsSwaping.Value = 1;
                if (_isFirstClick)
                {
                    _isFirstClick = false;
                    HandObj.transform.DOLocalMove(new Vector3(225, 150, 0), 0.35f).SetEase(Ease.InOutBack);
                    ColliderThree.enabled = true;
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
