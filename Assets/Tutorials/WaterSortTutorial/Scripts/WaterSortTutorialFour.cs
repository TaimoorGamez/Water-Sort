using Core.Events;
using DG.Tweening;
using UnityEngine;
using Core.Variables;
using System.Collections;
using Core.GamePlay.Coloring;

namespace Core.GamePlay.WaterSort
{
    public class WaterSortTutorialFour : MonoBehaviour
    {
        [SerializeField] SOEvents SwipeColorsModeEvent, StartColoringEvent;
        [SerializeField] SOInterger CanPlay, IsSwaping, UsingAnyFeature, LevelCompleteStateIndex;
        [SerializeField] SOIntegerEvents ChangeStateEvent;
        [SerializeField] CapsuleCollider ColliderOne, ColliderTwo, ColliderThree, ColliderFour, ColliderFive;
        [SerializeField] TubeHandler FirstTube, SecondTube, ThirdTube, ForthTube, FifthTube, SixthTube;
        [SerializeField] GameObject HandObj, InfoTextObj;
        [SerializeField] Color[] CurrentColors;
        [SerializeField] bool IsSwipButton, IsFirstTube, IsLastTube;
        [SerializeField] BowlColorHandler BowlObj;
        [SerializeField] Transform BowlParent;

        Vector3 _bowlScale = new Vector3(1.5f, 0.1f, 1.5f);
        int _colorIndex = 0, _colorBowlCounter = 0;
        Vector2 _bowlYPos = new Vector2(-2, -0.5f);
        bool _isFirstClick = true;

        private void OnEnable()
        {
            if (IsSwipButton)
            {
                ChangeStateEvent.EventHandler += HideInfoText;
                StartColoringEvent.EventHandler += ColoringPreparation;
            }
        }

        private void OnDisable()
        {
            if (IsSwipButton)
            {
                ChangeStateEvent.EventHandler -= HideInfoText;
                StartColoringEvent.EventHandler -= ColoringPreparation;
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
                FirstTube.SetColor(CurrentColors[_colorIndex]);
                yield return new WaitForSeconds(0.1f);
                if (_colorIndex == 3)
                {
                    _colorIndex = 0;
                }
                else
                {
                    _colorIndex++;
                }
                SecondTube.SetColor(CurrentColors[_colorIndex]);
                yield return new WaitForSeconds(0.1f); 
                if (_colorIndex == 3)
                {
                    _colorIndex = 0;
                }
                else
                {
                    _colorIndex++;
                }
                ThirdTube.SetColor(CurrentColors[_colorIndex]);
                yield return new WaitForSeconds(0.1f); 
                if (_colorIndex == 3)
                {
                    _colorIndex = 0;
                }
                else
                {
                    _colorIndex++;
                }
                ForthTube.SetColor(CurrentColors[_colorIndex]);
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

        public void SwapeColor()
        {
            if (UsingAnyFeature.Value == 0 && IsSwaping.Value == 0)
            {
                UsingAnyFeature.Value = 1;
                SwipeColorsModeEvent.InvokeSOEvent();
                IsSwaping.Value = 1;
                if (_isFirstClick)
                {
                    _isFirstClick = false;
                    HandObj.transform.DOLocalMove(new Vector3(225, 0, 0), 0.35f).SetEase(Ease.InOutBack);
                    ColliderThree.enabled = true;
                }
            }
        }

        void HideInfoText(int stateNum)
        {
            if (LevelCompleteStateIndex.Value == stateNum)
                InfoTextObj.SetActive(false);
        }

        void ColoringPreparation()
        {
            float tweenTime = 1;
            InfoTextObj.SetActive(false);
            if (FirstTube.WaterColors.Count > 0)
            {
                FirstTube.TubeCap.gameObject.SetActive(false);
                FirstTube.transform.DOScale(_bowlScale, tweenTime);
                FirstTube.transform.DOLocalMove(_bowlYPos, tweenTime).OnComplete(() =>
                {
                    BowlColorHandler colorBowl = Instantiate(BowlObj, BowlParent);
                    colorBowl.transform.localPosition = FirstTube.transform.localPosition;
                    colorBowl.SetColor(FirstTube.CurrentColor);
                    Destroy(FirstTube.gameObject);
                });

                if(_colorBowlCounter == 5)
                {
                    _bowlYPos = new Vector2(-2, -2);
                }
                else
                {
                    _bowlYPos += new Vector2(1, 0);
                }
            }
            else
            {
                FirstTube.transform.DOLocalMove(FirstTube.transform.position*5, tweenTime).OnComplete(() =>
                {
                    Destroy(FirstTube.gameObject);
                });
            }

            if (SecondTube.WaterColors.Count > 0)
            {
                SecondTube.TubeCap.gameObject.SetActive(false);
                SecondTube.transform.DOScale(_bowlScale, tweenTime);
                SecondTube.transform.DOLocalMove(_bowlYPos, tweenTime).OnComplete(() =>
                {
                    BowlColorHandler colorBowl = Instantiate(BowlObj, BowlParent);
                    colorBowl.transform.localPosition = SecondTube.transform.localPosition;
                    colorBowl.SetColor(SecondTube.CurrentColor);
                    Destroy(SecondTube.gameObject);
                });

                if (_colorBowlCounter == 5)
                {
                    _bowlYPos = new Vector2(-2, -2);
                }
                else
                {
                    _bowlYPos += new Vector2(1, 0);
                }
            }
            else
            {
                SecondTube.transform.DOLocalMove(SecondTube.transform.position * 5, tweenTime).OnComplete(() =>
                {
                    Destroy(SecondTube.gameObject);
                });
            }

            if (ThirdTube.WaterColors.Count > 0)
            {
                ThirdTube.TubeCap.gameObject.SetActive(false);
                ThirdTube.transform.DOScale(_bowlScale, tweenTime);
                ThirdTube.transform.DOLocalMove(_bowlYPos, tweenTime).OnComplete(() =>
                {
                    BowlColorHandler colorBowl = Instantiate(BowlObj, BowlParent);
                    colorBowl.transform.localPosition = ThirdTube.transform.localPosition;
                    colorBowl.SetColor(ThirdTube.CurrentColor);
                    Destroy(ThirdTube.gameObject);
                });

                if (_colorBowlCounter == 5)
                {
                    _bowlYPos = new Vector2(-2, -2);
                }
                else
                {
                    _bowlYPos += new Vector2(1, 0);
                }
            }
            else
            {
                ThirdTube.transform.DOLocalMove(ThirdTube.transform.position * 5, tweenTime).OnComplete(() =>
                {
                    Destroy(ThirdTube.gameObject);
                });
            }

            if (ForthTube.WaterColors.Count > 0)
            {
                ForthTube.TubeCap.gameObject.SetActive(false);
                ForthTube.transform.DOScale(_bowlScale, tweenTime);
                ForthTube.transform.DOLocalMove(_bowlYPos, tweenTime).OnComplete(() =>
                {
                    BowlColorHandler colorBowl = Instantiate(BowlObj, BowlParent);
                    colorBowl.transform.localPosition = ForthTube.transform.localPosition;
                    colorBowl.SetColor(ForthTube.CurrentColor);
                    Destroy(ForthTube.gameObject);
                });

                if (_colorBowlCounter == 5)
                {
                    _bowlYPos = new Vector2(-2, -2);
                }
                else
                {
                    _bowlYPos += new Vector2(1, 0);
                }
            }
            else
            {
                ForthTube.transform.DOLocalMove(ForthTube.transform.position * 5, tweenTime).OnComplete(() =>
                {
                    Destroy(ForthTube.gameObject);
                });
            }

            if (FifthTube.WaterColors.Count > 0)
            {
                FifthTube.TubeCap.gameObject.SetActive(false);
                FifthTube.transform.DOScale(_bowlScale, tweenTime);
                FifthTube.transform.DOLocalMove(_bowlYPos, tweenTime).OnComplete(() =>
                {
                    BowlColorHandler colorBowl = Instantiate(BowlObj, BowlParent);
                    colorBowl.transform.localPosition = FifthTube.transform.localPosition;
                    colorBowl.SetColor(FifthTube.CurrentColor);
                    Destroy(FifthTube.gameObject);
                });

                if (_colorBowlCounter == 5)
                {
                    _bowlYPos = new Vector2(-2, -2);
                }
                else
                {
                    _bowlYPos += new Vector2(1, 0);
                }
            }
            else
            {
                FifthTube.transform.DOLocalMove(FifthTube.transform.position * 5, tweenTime).OnComplete(() =>
                {
                    Destroy(FifthTube.gameObject);
                });
            }

            if (SixthTube.WaterColors.Count > 0)
            {
                SixthTube.TubeCap.gameObject.SetActive(false);
                SixthTube.transform.DOScale(_bowlScale, tweenTime);
                SixthTube.transform.DOLocalMove(_bowlYPos, tweenTime).OnComplete(() =>
                {
                    BowlColorHandler colorBowl = Instantiate(BowlObj, BowlParent);
                    colorBowl.transform.localPosition = SixthTube.transform.localPosition;
                    colorBowl.SetColor(SixthTube.CurrentColor);
                    Destroy(SixthTube.gameObject);
                });

                if (_colorBowlCounter == 5)
                {
                    _bowlYPos = new Vector2(-2, -2);
                }
                else
                {
                    _bowlYPos += new Vector2(1, 0);
                }
            }
            else
            {
                SixthTube.transform.DOLocalMove(SixthTube.transform.position * 5, tweenTime).OnComplete(() =>
                {
                    Destroy(SixthTube.gameObject);
                });
            }
        }

    }
}
